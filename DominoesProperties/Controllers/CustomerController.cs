using DominoesProperties.Helper;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Http;
using DominoesProperties.Services;
using System.Linq;
using System.Collections.Generic;
using StackExchange.Redis;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ILoggerManager logger;
        private readonly ICustomerRepository customerRepository;
        private readonly IStringLocalizer<CustomerController> localizer;
        private readonly IConnectionMultiplexer distributedCache;

        private readonly IConfiguration configuration;
        private readonly IApplicationSettingsRepository applicationSettingsRepository;
        private readonly IWebHostEnvironment environment;
        private readonly IAdminRepository adminRepository;
        private readonly IEmailService emailService;
        private readonly IInvestmentRepository investmentRepository;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");

        public CustomerController(ILoggerManager _logger, ICustomerRepository _customerRepository, IStringLocalizer<CustomerController> _stringLocalizer,
            IConnectionMultiplexer _distributedCache, IConfiguration _configuration, IApplicationSettingsRepository _applicationSettingsRepository,
            IWebHostEnvironment _environment, IAdminRepository _adminRepository, IEmailService _emailService, IInvestmentRepository _investmentRepository)
        {
            logger = _logger;
            customerRepository = _customerRepository;
            localizer = _stringLocalizer;
            distributedCache = _distributedCache;
            configuration = _configuration;
            adminRepository = _adminRepository;
            investmentRepository = _investmentRepository;
            applicationSettingsRepository = _applicationSettingsRepository;
            environment = _environment;
            emailService = _emailService;
        }

        [HttpPost]
        [Route("register")]
        public ApiResponse RegisterAsync([FromBody] CustomerReq customer)
        {
            if (customerRepository.GetCustomer(customer.Email) != null)
            {
                response.Message = $"Customer with email {customer.Email} already exist";
                return response;
            }

            if (customerRepository.GetCustomer(customer.Phone) != null)
            {
                response.Message = $"Customer with phone numer {customer.Phone} already exist";
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
        public ApiResponse Login(Login login)
        {
            var customer = customerRepository.GetCustomer(login.Email);
            if (customer == null || customer.IsDeleted.Value)
            {
                response.Success = false;
                response.Message = "Username name not found!";
                return response;
            }

            if (!customer.IsVerified.Value)
            {
                response.Success = false;
                response.Message = $"Customer account not verified, kindly check your email to verify your account or <html><a href='{configuration.GetValue<string>("app_settings:ApiEndpoint")}/customer/resend/{customer.UniqueRef}' style='color:blue;'>Click Here</a></html> to resend verification email";
                return response;
            }

            if (customer.Password.Equals(CommonLogic.Encrypt(login.Password)))
            {
                response.Data = ClassConverter.ConvertCustomerToProfile(customer);
                response.Success = true;
                response.Message = "Login successful";
                Response.Headers.Add("access_token", GenerateJwtToken(customer.UniqueRef));
                return response;
            }
            else
            {
                response.Success = false;
                response.Message = "Incorrect password supplied";
                return response;
            }
        }

        [HttpDelete]
        [Authorize(Roles = "SUPER, ADMIN")]
        public ApiResponse Delete()
        {
            customerRepository.DeleteCustomer(HttpContext.User.Identity.Name);
            response.Success = true;
            response.Message = response.Message = "Customer successfully deleted";
            return response;
        }

        [HttpPut]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse Update([FromBody] CustomerUpdate customer)
        {
            var uniqueRef = HttpContext.User.Identity.Name;
            var existingCustomer = customerRepository.GetCustomer(uniqueRef);
            if (existingCustomer == null)
            {
                response.Message = $"Customer with id {uniqueRef} not found.";
                return response;
            }

            existingCustomer.Address = customer.Address;
            existingCustomer.Phone = customer.Phone;
            if (string.IsNullOrEmpty(existingCustomer.AccountNumber))
            {
                existingCustomer.AccountNumber = customer.AccountNumber;
                existingCustomer.BankName = customer.BankName;
            }

            response.Message = "Customer profile updated successfully!";
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

            _ = ActivationLink(uniqueRef, ValidationModule.ACTIVATE_ACCOUNT, setting);

            response.Message = "Activation link successfully generated and sent to customer email, kindly check your email to activate account";
            response.Success = true;
            return response;
        }

        [HttpGet]
        [Route("resend/{uniqueRef}")]
        [AllowAnonymous]
        public RedirectResult ResendActivationLink(string uniqueRef)
        {
            ApplicationSetting setting = applicationSettingsRepository.GetApplicationSettingsByName("EmailNotification");

            _ = ActivationLink(uniqueRef, ValidationModule.ACTIVATE_ACCOUNT, setting);

            return Redirect($"{configuration["app_settings:WebEndpoint"]}?activation-status=success");
        }

        [HttpPut]
        [Route("activate/{token}")]
        [AllowAnonymous]
        public async Task<ApiResponse> Activate(string token)
        {
            var db = distributedCache.GetDatabase();
            var uniqueRef = await db.StringGetAsync(token);
            if (!string.IsNullOrEmpty(uniqueRef))
            {
                var customer = customerRepository.GetCustomer(uniqueRef);
                customer.IsVerified = true;
                customer.IsActive = true;
                if (customerRepository.UpdateCustomer(customer) != null)
                {
                    string filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\welcome.html");
                    string html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                    html = html.Replace("{FIRSTNAME}", string.Format("{0} {1}", customer.FirstName, customer.LastName)).Replace("{webroot}", configuration["app_settings:WebEndpoint"]); ;

                    EmailData emailData = new()
                    {
                        EmailBody = html,
                        EmailSubject = "Welcome!",
                        EmailToId = customer.Email,
                        EmailToName = customer.FirstName
                    };
                    emailService.SendEmail(emailData);

                    response.Message = string.Format("Customer account {0} successfully activated", customer.Email);
                    response.Success = true;
                    response.Data = ClassConverter.ConvertCustomerToProfile(customer);
                    return response;
                }
            }
            return response;
        }

        [HttpGet]
        [Authorize]
        public ApiResponse Customer()
        {
            var customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
            if (customer != null)
            {
                response.Data = ClassConverter.ConvertCustomerToFullProfile(customer);
                response.Message = "Successfuly fetch customer";
                response.Success = true;
                return response;
            }
            else
            {
                response.Message = "Invalid username provided";
            }
            return response;
        }

        [HttpGet("reset-password/{email}")]
        [AllowAnonymous]
        public ApiResponse ResetPassword(string email)
        {
            ApplicationSetting settings = applicationSettingsRepository.GetApplicationSettingsByName("EmailNotification");

            _ = ActivationLink(email, ValidationModule.RESET_PASSWORD, settings);
            
            response.Message = string.Format("Password reset link successfully sent to {0}", email);
            response.Success = true;
            return response;
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ApiResponse> ResetPasswordConfirm([FromBody] PasswordReset password)
        {
            var db = distributedCache.GetDatabase();
            var uniqueRef = await db.StringGetAsync(password.Token);
            if (!string.IsNullOrEmpty(uniqueRef))
            {
                var customer = customerRepository.GetCustomer(uniqueRef);
                customer.Password = password.Password;
                customer.IsActive = true;
                customer.IsVerified = true;
                customerRepository.UpdateCustomer(customer);

                response.Message = string.Format("Password reset successful for {0}", customer.Email);
                response.Success = true;
                response.Data = ClassConverter.ConvertCustomerToProfile(customer);
                return response;
            }
            else
            {
                response.Message = "Invalid username supplied";
                return response;
            }
        }

        [HttpPost("change-password")]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse ChangePassword([FromBody] PasswordReset password)
        {
            var customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
            if (customer == null)
            {
                response.Message = "Invalid username supplied";
                return response;
            }

            if (!customer.Password.Equals(CommonLogic.Encrypt(password.Token)))
            {
                response.Message = "Invalid old password supplied, kindly check and try again";
                return response;
            }
            customer.Password = CommonLogic.Encrypt(password.Password);
            customerRepository.UpdateCustomer(customer);

            response.Message = string.Format("Password reset successful for {0}", customer.Email);
            response.Success = true;
            return response;
        }

        [HttpPost("passport")]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse UploadFile([FromBody] string UploadUrl)
        {
            var customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
            if(customer == null)
            {
                response.Message = "Customer not found, please login with your credentials and try again";
                return response;
            }
            customer.PassportUrl = UploadUrl;
            if(customerRepository.UpdateCustomer(customer) != null)
            {
                response.Success = true;
                response.Message = "Passport successfully uploaded";
                return response;
            }
            response.Message = "Error uploading passport photo, please try again later";
            return response;
        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse Dashboard()
        {
            var customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
            var investment = investmentRepository.GetInvestments(customer.Id);
            Dictionary<string, int> dashboardElement = new();
            dashboardElement.Add("TotalInvestment", investment.Count);

            var result = (from item in investment
                          group item by item.Property.Status into g
                          select new InvestCat() { Status = g.Key, Values = g.Count() }).ToList();

            var e = result.Where(x => x.Status.Equals("OPEN_FOR_INVESTMENT") || x.Status.Equals("ONGOING_CONSTRUCTION")).Sum(x => x.Values);
            var f = result.Where(x => x.Status.Equals("CLOSED_FOR_INVESTMENT") || x.Status.Equals("RENTED_OUT")).Sum(x => x.Values);

            dashboardElement.Add("ActiveInvestment", e);
            dashboardElement.Add("ClosedInvestment", f);

            response.Message = "Succcessfull";
            response.Success = true;
            response.Data = dashboardElement;

            return response;
        }

        private async Task<bool> ActivationLink(string uniqueRef, ValidationModule validationModule, ApplicationSetting setting)
        {
            var customer = customerRepository.GetCustomer(uniqueRef);
            if (customer != null)
            {
                try
                {
                    string token = "", html = "", subject = "", filePath="", url;
                    switch (validationModule)
                    {
                        case ValidationModule.ACTIVATE_ACCOUNT:
                            token = CommonLogic.GetUniqueRefNumber("AT");
                            url = string.Format("{0}/home/{1}/{2}?value={3}", configuration["app_settings:WebEndpoint"], validationModule.ToString().ToLower(), token, "customer");
                            filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\activation.html");
                            html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                            html = html.Replace("{FIRSTNAME}", string.Format("{0} {1}", customer.FirstName, customer.LastName)).Replace("{LINK}", url).Replace("{webroot}", configuration["app_settings:WebEndpoint"]);
                            subject = "Real Estate Dominoes - Account Activation";
                            break;
                        case ValidationModule.RESET_PASSWORD:
                            token = CommonLogic.GetUniqueRefNumber("RS");
                            url = string.Format("{0}/home/{1}/{2}?value={3}", configuration["app_settings:WebEndpoint"], validationModule.ToString().ToLower(), token, "customer");
                            filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\password-reset.html");
                            html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                            html = html.Replace("{FIRSTNAME}", string.Format("{0} {1}", customer.FirstName, customer.LastName)).Replace("{LINK}", url).Replace("{webroot}", configuration["app_settings:WebEndpoint"]);
                            subject = "Real Estate Dominoes - Reset Account Password";
                            break;
                        default:
                            break;
                    }

                    var db = distributedCache.GetDatabase();
                    _ = await db.StringSetAsync(token, uniqueRef, TimeSpan.FromMinutes(30));

                    EmailData emailData = new()
                    {
                        EmailBody = html,
                        EmailSubject = subject,
                        EmailToId = customer.Email,
                        EmailToName = customer.FirstName
                    };
                    emailService.SendEmail(emailData);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.StackTrace);
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
                new Claim(JwtRegisteredClaimNames.UniqueName, uniqueRef),
                new Claim(JwtRegisteredClaimNames.Jti, Convert.ToString(Guid.NewGuid())),
                new Claim(ClaimTypes.Role, "CUSTOMER")
            };

            var token = new JwtSecurityToken(configuration["app_settings:Issuer"],
               configuration["app_settings:Issuer"], claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

internal class InvestCat
{
    public string Status { get; set; }
    public int Values { get; set; }
}