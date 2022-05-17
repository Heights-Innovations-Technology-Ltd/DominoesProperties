using DominoesProperties.Helper;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Repositories.Repository;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using DominoesProperties.Enums;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ILoggerManager logger;
        private readonly ICustomerRepository customerRepository;
        private readonly IStringLocalizer<CustomerController> localizer;
        private readonly IDistributedCache distributedCache;
        private readonly IConfiguration configuration;
        private readonly IApplicationSettingsRepository applicationSettingsRepository;
        private readonly IHostingEnvironment environment;
        private readonly ApiResponse response = new ApiResponse(false, "Error performing request, contact admin");
        private readonly DistributedCacheEntryOptions expiryOptions;

        public CustomerController(ILoggerManager _logger, ICustomerRepository _customerRepository, IStringLocalizer<CustomerController> _stringLocalizer,
            IDistributedCache _distributedCache, IConfiguration _configuration, IApplicationSettingsRepository _applicationSettingsRepository, IHostingEnvironment _environment)
        {
            logger = _logger;
            customerRepository = _customerRepository;
            localizer = _stringLocalizer;
            distributedCache = _distributedCache;
            configuration = _configuration;
            expiryOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20),
                SlidingExpiration = TimeSpan.FromMinutes(15)
            };
            applicationSettingsRepository = _applicationSettingsRepository;
            environment = _environment;
        }

        [HttpPost]
        [Route("register")]
        public ApiResponse RegisterAsync([FromBody] Models.Customer customer)
        {
            var customerReg = customerRepository.CreateCustomer(ClassConverter.ConvertCustomerToEntity(customer));
            var setting = applicationSettingsRepository.GetApplicationSettingsByName("EmailNotification");
            if (customerReg != null)
            {
                _ = ActivationLink(customer.Email, ValidationModule.ACTIVATE_ACCOUNT, setting);

                response.Success = true;
                response.Message = localizer["Response.Created"].Name.Replace("{params}", "Customer");
                logger.LogInfo(response.Message);
                return response;
            }
            return response;
        }

        [HttpPost]
        [Route("login")]
        public ApiResponse Login([FromBody] Login login)
        {
            var customer = customerRepository.GetCustomer(login.Email);
            if (!customer.IsVerified.Value)
            {
                response.Success = false;
                response.Message = localizer["Customer.NotVerified"];
                return response;
            }
            if (customer == null || !customer.IsActive.Value || customer.IsDeleted.Value)
            {
                response.Success = false;
                response.Message = localizer["Username.Error"];
                return response;
            }

            if (customer.Password.Equals(CommonLogic.Encrypt(login.Password)))
            {
                response.Data = ClassConverter.ConvertCustomerToProfile(customer);
                response.Success = true;
                response.Message = response.Message = localizer["Response.Success"];
                Response.Headers.Add("access_token", GenerateJwtToken(customer.UniqueRef));
                return response;
            }
            else
            {
                response.Success = false;
                response.Message = localizer["Password.Error"];
                return response;
            }
        }

        [HttpDelete]
        [Authorize]
        public ApiResponse Delete()
        {
            customerRepository.DeleteCustomer(HttpContext.User.Identity.Name);
            response.Success = true;
            response.Message = response.Message = localizer["Response.Success"];
            return response;
        }

        [HttpPut]
        [Authorize]
        public ApiResponse Update([FromBody] Models.Customer customer)
        {
            var uniqueRef = HttpContext.User.Identity.Name;
            var existingCustomer = customerRepository.GetCustomer(uniqueRef);
            if (existingCustomer == null)
            {
                response.Message = $"Customer with id {uniqueRef} not found.";
                return response;
            }

            existingCustomer.Address = customer.Address;
            existingCustomer.AccountNumber = customer.AccountNumber;
            existingCustomer.Phone = customer.Phone;

            response.Message = localizer["Response.Success"];
            response.Success = true;
            response.Data = customerRepository.UpdateCustomer(existingCustomer);
            return response;
        }

        [HttpGet]
        [Route("activate/{uniqueRef}")]
        [AllowAnonymous]
        public ApiResponse SendActivationLink(string uniqueRef)
        {
            ApplicationSetting setting = applicationSettingsRepository.GetApplicationSettingsByName("EmailNotification");
            if (ActivationLink(uniqueRef, ValidationModule.ACTIVATE_ACCOUNT, setting).IsCompleted)
            {
                response.Message = localizer["Auth.Link.Generated"];
                response.Success = true;
                return response;
            }
            else
            {
                response.Message = localizer["Username.Error"];
                return response;
            }
        }

        [HttpPut]
        [Route("activate/{token}")]
        [AllowAnonymous]
        public async Task<ApiResponse> Activate(string token)
        {
            var uniqueRef = await distributedCache.GetStringAsync(token);
            if (!string.IsNullOrEmpty(uniqueRef))
            {
                var customer = customerRepository.GetCustomer(uniqueRef);
                customer.IsVerified = true;
                customer.IsActive = true;
                customerRepository.UpdateCustomer(customer);

                response.Message = string.Format(localizer["Response.Customer.Activated"], customer.Email);
                response.Success = true;
                response.Data = ClassConverter.ConvertCustomerToProfile(customer);
                return response;
            }
            return response;
        }

        [HttpGet]
        [Authorize]
        public ApiResponse Customer()
        {
            var customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
            if (customer != null) {
                response.Data = ClassConverter.ConvertCustomerToFullProfile(customer);
                response.Message = localizer["Response.Success"];
                response.Success = true;
                return response;
            }
            else
            {
                response.Message = localizer["Username.Error"];
            }
            return response;
        }



        [HttpGet("reset-password/{email}")]
        [AllowAnonymous]
        public ApiResponse ResetPassword(string email)
        {
            ApplicationSetting settings = applicationSettingsRepository.GetApplicationSettingsByName("EmailNotification");

            if (ActivationLink(email, ValidationModule.RESET_PASSWORD, settings).IsCompleted)
            {
                response.Message = string.Format(localizer["Response.Customer.Password.Link"], email);
                response.Success = true;
                return response;
            }
            else
            {
                response.Message = localizer["Username.Error"];
            }
            return response;
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ApiResponse> ResetPasswordConfirm([FromBody]PasswordReset password)
        {
            var uniqueRef = await distributedCache.GetStringAsync(password.Token);
            if (!string.IsNullOrEmpty(uniqueRef))
            {
                var customer = customerRepository.GetCustomer(uniqueRef);
                customer.Password = password.Password;
                customer.IsActive = true;
                customer.IsVerified = true;
                customerRepository.UpdateCustomer(customer);

                response.Message = string.Format(localizer["Response.Customer.Password.Reset"], customer.Email);
                response.Success = true;
                response.Data = ClassConverter.ConvertCustomerToProfile(customer);
                return response;
            }
            else
            {
                response.Message = localizer["Username.Error"];
                return response;
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public ApiResponse ChangePassword([FromBody] PasswordReset password)
        {
            var customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
            if(customer == null)
            {
                response.Message = localizer["Username.Error"];
                return response;
            }

            if (!customer.Password.Equals(CommonLogic.Encrypt(password.Token)))
            {
                response.Message = localizer["Response.Customer.Password.Invalid"];
                return response;
            }
            customer.Password = password.Password;
            customerRepository.UpdateCustomer(customer);

            response.Message = string.Format(localizer["Response.Customer.Password.Reset"], customer.Email);
            response.Success = true;
            return response;
        }

        private async Task<bool> ActivationLink(string uniqueRef, ValidationModule validationModule, ApplicationSetting setting)
        {
            var customer = customerRepository.GetCustomer(uniqueRef);
            if (customer != null)
            {
                try
                {
                    string token = "", html = "";

                    switch (validationModule)
                    {
                        case ValidationModule.ACTIVATE_ACCOUNT:
                            token = CommonLogic.GetUniqueRefNumber("AT");
                            string url = string.Format("{0}{1}/{2}", configuration["app_settings:WebEndpoint"], validationModule.ToString().ToLower(), token);
                            string filePath = System.IO.Path.Combine(environment.ContentRootPath, @"EmailTemplates\NewCustomer.html");
                            html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                            html = html.Replace("{name}", string.Format("{0} {1}", customer.FirstName, customer.LastName).Replace("{link}", url));
                            break;
                        case ValidationModule.RESET_PASSWORD:
                            token = CommonLogic.GetUniqueRefNumber("RS");
                            break;
                        default:
                            break;
                    }

                
                    await distributedCache.SetStringAsync(token, uniqueRef, expiryOptions);

                    //TODO create Html template for registration with url embedded
                
                    EmailRequest emailRequest = new(localizer["Customer.Registration.Subject"], html, customer.Email);
                    emailRequest.Settings = setting;
                    CommonLogic.SendEmail(emailRequest);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.InnerException.ToString());
                }

                return true;
            }
            return false;
        }

        private string GenerateJwtToken(string uniqueRef)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["app_settings:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.UniqueName, uniqueRef),new Claim(JwtRegisteredClaimNames.Jti, Convert.ToString(Guid.NewGuid()))
            };

            var token = new JwtSecurityToken(configuration["app_settings:Issuer"],
               configuration["app_settings:Issuer"], claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
