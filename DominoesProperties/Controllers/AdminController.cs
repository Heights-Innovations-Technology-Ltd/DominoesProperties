using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DominoesProperties.Enums;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using DominoesProperties.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAdminRepository adminRepository;
        private readonly IConfiguration configuration;
        private readonly ICustomerRepository customerRepository;
        private readonly IDistributedCache distributedCache;
        private readonly IWebHostEnvironment environment;
        private readonly DistributedCacheEntryOptions expiryOptions;
        private readonly ILoggerManager logger;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");

        public AdminController(IAdminRepository _adminRepository, IConfiguration _configuration,
            IWebHostEnvironment _environment, IDistributedCache _distributedCache,
            ICustomerRepository _customerRepository, ILoggerManager _logger, IInvestmentRepository investmentRepository,
            IPropertyRepository propertyRepository,
            ITransactionRepository transactionRepository, IEmailService emailService)
        {
            adminRepository = _adminRepository;
            configuration = _configuration;
            environment = _environment;
            distributedCache = _distributedCache;
            customerRepository = _customerRepository;
            logger = _logger;
            _investmentRepository = investmentRepository;
            _propertyRepository = propertyRepository;
            _transactionRepository = transactionRepository;
            _emailService = emailService;
            expiryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20),
                SlidingExpiration = TimeSpan.FromMinutes(15)
            };
        }

        [HttpPost]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> AdminAsync([FromHeader] string apiKey, [FromHeader] string adminUsername,
            [FromBody] AdminUser admin)
        {
            if (string.IsNullOrEmpty(apiKey) ||
                !")H@McQfTjWnZr4t7w!z%C*F-JaNdRgUkXp2s5v8x/A?D(G+KbPeShVmYq3t6w9z$".Equals(apiKey))
            {
                throw new UnauthorizedAccessException("Unauthorised user access, kindly contact admin");
            }

            if (adminRepository.GetUser(adminUsername).RoleFk != (int)Enums.Role.SUPER)
            {
                response.Message = $"Invalid admin user {admin.Email}";
                return response;
            }

            if (adminRepository.GetUser(admin.Email) != null)
            {
                response.Message = $"User exist with email {admin.Email}";
                return response;
            }

            if (customerRepository.GetCustomers().Exists(x => x.Email.Equals(admin.Email)))
            {
                response.Message = $"Customer exist with email {admin.Email} and a customer cannot be admin";
                return response;
            }

            var adminEntity = ClassConverter.UserToAdmin(admin);
            adminEntity.CreatedBy = adminUsername;
            if (adminRepository.AddUser(adminEntity))
            {
                try
                {
                    var token = Guid.NewGuid().ToString();
                    var url =
                        $"{configuration["app_settings:WebEndpoint"]}{ValidationModule.ACTIVATE_ACCOUNT}/{token}?value={"admin"}";
                    var filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\NewCustomer.html");
                    var html = await System.IO.File.ReadAllTextAsync(filePath.Replace(@"\", "/"));
                    html = html.Replace("{name}", admin.Email).Replace("{link}", HttpUtility.UrlEncode(url));

                    await distributedCache.SetStringAsync(token, admin.Email, expiryOptions);

                    EmailRequest emailRequest = new("Dominoes Society - Account Creation", html, admin.Email)
                    {
                        Settings = new ApplicationSetting
                            { TestingMode = false, SettingName = "EmailNotification" }
                    };
                    CommonLogic.SendEmail(emailRequest);
                }
                catch (Exception ex)
                {
                    _ = new ExceptionFormatter(logger, ex);
                }
            }

            response.Message = $"Admin user {admin.Email} created successfully";
            return response;
        }

        [HttpPost("login")]
        public ApiResponse Login([FromBody] Login login)
        {
            Admin admin = adminRepository.GetUser(login.Email);
            if (admin == null)
            {
                response.Success = false;
                response.Message = $"Invalid username {login.Email} or password supplied";
                return response;
            }

            if (!admin.IsActive.Value)
            {
                response.Success = false;
                response.Message =
                    $"<html>User not verified, <b><a href='{environment.WebRootPath}/customer/activate/{login.Email}'>click here</a></b> to resend verification link</html>";
                return response;
            }

            if (admin == null || admin.IsDeleted.Value)
            {
                response.Success = false;
                response.Message = "Login error: the username supplied is invalid";
                return response;
            }

            if (admin.Password.Equals(CommonLogic.Encrypt(login.Password)))
            {
                response.Data = admin;
                response.Success = true;
                response.Message = response.Message = $"Welcome back! {login.Email}";
                Response.Headers.Add("access_token", GenerateJwtToken(admin.Email));
                return response;
            }
            else
            {
                response.Success = false;
                response.Message = "Login error: incorrect password supplied";
                return response;
            }
        }

        [HttpPut("activate/{token}")]
        public async Task<ApiResponse> AdminAsync(string token)
        {
            var uniqueRef = await distributedCache.GetStringAsync(token);
            if (string.IsNullOrEmpty(uniqueRef)) return response;
            var admin = adminRepository.GetUser(uniqueRef);
            admin.IsActive = true;
            adminRepository.UpdateUser(admin);

            response.Message = $"User {uniqueRef} successfully activated, kindly login to access the application";
            response.Success = true;
            return response;
        }

        [HttpDelete("{email}")]
        [Authorize(Roles = "SUPER", Policy = "Super")]
        public ApiResponse DeleteUser([EmailAddress(ErrorMessage = "Not a valid email address")] string email)
        {
            var admin = adminRepository.GetUser(email);
            if (admin is { IsDeleted: false })
            {
                admin.IsDeleted = true;
                admin.IsActive = false;
                adminRepository.UpdateUser(admin);

                response.Message = $"User {email} successfully deleted and deactivated";
                response.Success = true;
                return response;
            }
            else
            {
                response.Message = $"User {email} cannot be found or is already deleted";
                return response;
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public ApiResponse ResetPassword([FromBody] ChangePassword changePassword)
        {
            var admin = adminRepository.GetUser(HttpContext.User.Identity.Name);
            admin.Password = CommonLogic.Encrypt(changePassword.Password);
            adminRepository.UpdateUser(admin);

            response.Message = $"Password successfully changed.";
            response.Success = true;
            return response;
        }

        [HttpGet("dashboard")]
        [Authorize]
        public ApiResponse Dashboard()
        {
            response.Message = $"Password successfully changed.";
            response.Data = adminRepository.AdminDashboard();
            response.Success = true;
            return response;
        }

        [HttpGet("offline-payment")]
        [Authorize(Roles = "ADMIN, SUPER")]
        public async Task<ApiResponse> OfflinePayment()
        {
            var inv = await _investmentRepository.GetOfflineInvestments();

            if (inv != null)
            {
                response.Message = $"Offline investments fetched.";
                response.Data = inv;
                response.Success = true;
                return response;
            }

            response.Message = $"No record found.";
            return response;
        }

        [HttpPut("offline-payment/{investmentRef}")]
        [Authorize(Roles = "ADMIN, SUPER")]
        public ApiResponse CompleteOfflinePayment([FromBody] OfflineResponse offlineResponse, string investmentRef)
        {
            string filePath = "", html = "";
            EmailData emailData;
            var inv = _investmentRepository.GetOfflineInvestment(investmentRef);
            if (inv != null && (inv.Status.Equals(Status.PROCESSING.ToString()) ||
                                inv.Status.Equals(Status.DECLINED.ToString())))
            {
                var customer = customerRepository.GetCustomer(inv.CustomerId);
                inv.TreatedBy = HttpContext.User.Identity!.Name;
                inv.TreatedDate = DateTime.Now;
                inv.Status = offlineResponse.Status.ToString();
                inv.Comment = offlineResponse.Comment;

                switch (offlineResponse.Status)
                {
                    case Status.DECLINED:
                        filePath = Path.Combine(environment.ContentRootPath,
                            @"EmailTemplates\declined-investment.html");
                        html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                        html = html.Replace("{FIRSTNAME}", $"{customer.FirstName} {customer.LastName}")
                            .Replace("{webroot}", configuration["app_settings:WebEndpoint"]);

                        response.Message = "Investment declined and closed";
                        break;
                    case Status.APPROVED:
                    {
                        var prop = _propertyRepository.GetProperty(inv.PropertyId);
                        if (prop.UnitAvailable < inv.Units)
                        {
                            inv.Comment =
                                $"Not enough unit left on {prop.Name} to service investment request. Kindly process refund to customer wallet";
                            inv.Status = Status.DECLINED.ToString();
                            response.Message = inv.Comment;

                            filePath = Path.Combine(environment.ContentRootPath,
                                @"EmailTemplates\declined-investment.html");
                            html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                            html = html.Replace("{FIRSTNAME}", $"{customer.FirstName} {customer.LastName}")
                                .Replace("{webroot}", configuration["app_settings:WebEndpoint"]);
                        }
                        else
                        {
                            inv.Status = Status.APPROVED.ToString();
                            inv.Comment = "Investment booked successfully";

                            using (var scope =
                                   new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption
                                       .RequiresNew))
                            {
                                Transaction transaction = new()
                                {
                                    Amount = inv.Amount,
                                    Channel = Channel.OFFLINE.ToString(),
                                    Module = "PROPERTY_PURCHASE",
                                    Status = "success",
                                    CustomerId = inv.CustomerId,
                                    TransactionDate = inv.TreatedDate,
                                    TransactionType = TransactionType.CR.ToString(),
                                    TransactionRef = Guid.NewGuid().ToString()
                                };
                                _transactionRepository.NewTransaction(transaction);

                                Investment newInv = new()
                                {
                                    CustomerId = inv.CustomerId,
                                    Amount = inv.Amount,
                                    Status = Status.COMPLETED.ToString(),
                                    Units = inv.Units,
                                    PaymentDate = inv.PaymentDate!.Value,
                                    TransactionRef = transaction.TransactionRef,
                                    PropertyId = inv.PropertyId,
                                    PaymentType = Channel.OFFLINE.ToString(),
                                    UnitPrice = inv.UnitPrice!.Value,
                                    Yield = prop.TargetYield,
                                    YearlyInterestAmount = (prop.TargetYield * inv.Amount) / 100
                                };
                                _investmentRepository.AddInvestment(newInv);

                                prop.UnitSold += newInv.Units;
                                prop.UnitAvailable -= newInv.Units;
                                if (prop.UnitAvailable == 0)
                                    prop.Status = Status.CLOSED.ToString();
                                _propertyRepository.UpdateProperty(prop);

                                scope.Complete();

                                filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\investment.html");
                                html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                                html = html.Replace("{FIRSTNAME}", $"{customer.FirstName} {customer.LastName}")
                                    .Replace("{I-NAME}", prop.Name);
                                html = html.Replace("{I-UNITS}", inv.Units.ToString())
                                    .Replace("{I-PRICE}", inv.UnitPrice.ToString())
                                    .Replace("{I-TOTAL}", inv.Amount.ToString(CultureInfo.CurrentCulture))
                                    .Replace("{I-DATE}", inv.PaymentDate.ToString())
                                    .Replace("{webroot}", configuration["app_settings:WebEndpoint"]);
                            }

                            response.Message = $"Investment successfully completed";
                            response.Success = true;
                        }

                        break;
                    }
                }

                _investmentRepository.UpdateOfflineInvestment(inv);

                Task.Run(() =>
                {
                    emailData = new EmailData()
                    {
                        EmailBody = html,
                        EmailSubject = "Congratulations!!! You just made an investment",
                        EmailToId = customer.Email,
                        EmailToName = customer.FirstName
                    };
                    _emailService.SendEmail(emailData);
                });
            }
            else
                response.Message = $"Offline investment record not found or already treated.";

            return response;
        }

        private string GenerateJwtToken(string uniqueRef)
        {
            var admin = adminRepository.GetUser(uniqueRef);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["app_settings:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, uniqueRef),
                new Claim(JwtRegisteredClaimNames.Jti, uniqueRef),
                new Claim(ClaimTypes.Role, admin.RoleFkNavigation.RoleName)
            };

            var token = new JwtSecurityToken(configuration["app_settings:Issuer"],
                configuration["app_settings:Issuer"], claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}