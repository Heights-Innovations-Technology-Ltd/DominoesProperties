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
using System.Web;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Azure.Storage.Blobs.Models;

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
        private readonly IWebHostEnvironment environment;
        private readonly IAdminRepository adminRepository;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");
        private readonly DistributedCacheEntryOptions expiryOptions;

        public CustomerController(ILoggerManager _logger, ICustomerRepository _customerRepository, IStringLocalizer<CustomerController> _stringLocalizer,
            IDistributedCache _distributedCache, IConfiguration _configuration, IApplicationSettingsRepository _applicationSettingsRepository,
            IWebHostEnvironment _environment, IAdminRepository _adminRepository)
        {
            logger = _logger;
            customerRepository = _customerRepository;
            localizer = _stringLocalizer;
            distributedCache = _distributedCache;
            configuration = _configuration;
            adminRepository = _adminRepository;
            expiryOptions = new()
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
            if(customerRepository.GetCustomer(customer.Email) != null)
            {
                response.Message = $"Customer with email {customer.Email} already exist";
                return response;
            }

            if (adminRepository.GetUser(customer.Email) != null)
            {
                response.Message = $"Admin user exist with email {customer.Email} and admin is not allowed as a customer";
                return response;
            }

            var customerReg = customerRepository.CreateCustomer(ClassConverter.ConvertCustomerToEntity(customer));
            var setting = applicationSettingsRepository.GetApplicationSettingsByName("EmailNotification");
            if (customerReg != null)
            {
                _ = ActivationLink(customer.Email, ValidationModule.ACTIVATE_ACCOUNT, setting);

                response.Success = true;
                response.Message = "Customer account successfully created!";
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

        [HttpPost("passport")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ApiResponse> UploadPassportAsync([Required][MaxLength(1 * 1024 * 1024, ErrorMessage = "Upload size cannot exceed 1MB")]  IFormFile passport)
        {
            var container = new BlobContainerClient(configuration["BlobClient:Url"], "passport");
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);
            var blob = container.GetBlobClient($"{HttpContext.User.Identity.Name}.{passport.FileName[passport.FileName.LastIndexOf(".")..]}");

            using (var fileStream = passport.OpenReadStream())
            {
                await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = passport.ContentType });
            }
            response.Message = "Passport successfully uploaded";
            response.Data = blob.Uri.ToString();
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
                            string url = string.Format("{0}{1}/{2}?value={3}", configuration["app_settings:WebEndpoint"], validationModule.ToString().ToLower(), token, "customer");
                            string filePath = System.IO.Path.Combine(environment.ContentRootPath, @"EmailTemplates\NewCustomer.html");
                            html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                            html = html.Replace("{name}", string.Format("{0} {1}", customer.FirstName, customer.LastName).Replace("{link}", HttpUtility.UrlEncode(url)));
                            break;
                        case ValidationModule.RESET_PASSWORD:
                            token = CommonLogic.GetUniqueRefNumber("RS");
                            break;
                        default:
                            break;
                    }

                
                    await distributedCache.SetStringAsync(token, uniqueRef, expiryOptions);
                
                    EmailRequest emailRequest = new("New Customer -  Real Estate ", html, customer.Email);
                    emailRequest.Settings = setting;
                    CommonLogic.SendEmail(emailRequest);
                }
                catch (Exception ex)
                {
                    _ = new ExceptionFormatter(logger, ex);
                    logger.LogError(ex.InnerException.ToString());
                }

                return true;
            }
            return false;
        }

        protected string GenerateJwtToken(string uniqueRef)
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
