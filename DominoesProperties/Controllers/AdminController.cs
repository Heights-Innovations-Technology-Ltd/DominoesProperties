using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DominoesProperties.Enums;
using DominoesProperties.Helper;
using DominoesProperties.Models;
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
        private readonly IAdminRepository adminRepository;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");
        private readonly DistributedCacheEntryOptions expiryOptions;
        private readonly IDistributedCache distributedCache;
        private readonly ICustomerRepository customerRepository;
        private readonly ILoggerManager logger;

        public AdminController(IAdminRepository _adminRepository, IConfiguration _configuration, IWebHostEnvironment _environment, IDistributedCache _distributedCache,
            ICustomerRepository _customerRepository, ILoggerManager _logger)
        {
            adminRepository = _adminRepository;
            configuration = _configuration;
            environment = _environment;
            distributedCache = _distributedCache;
            customerRepository = _customerRepository;
            logger = _logger;
            expiryOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20),
                SlidingExpiration = TimeSpan.FromMinutes(15)
            };
        }

        [HttpPost]
        public async Task<ApiResponse> AdminAsync([FromHeader] string apiKey, [FromHeader] string adminUsername, [FromBody] AdminUser admin)
        {
            if(string.IsNullOrEmpty(apiKey) || !")H@McQfTjWnZr4t7w!z%C*F-JaNdRgUkXp2s5v8x/A?D(G+KbPeShVmYq3t6w9z$".Equals(apiKey))
            {
                throw new UnauthorizedAccessException("Unauthorised user access, kindly contact admin");
            }
            if (adminRepository.GetUser(adminUsername).RoleFk != (int) Enums.Role.SUPER)
            {
                response.Message = $"Invalid admin user {admin.Email}";
                return response;
            }
            if (adminRepository.GetUser(admin.Email) != null)
            {
                response.Message = $"User exist with email {admin.Email}";
                return response;
            }
            if(customerRepository.GetCustomers().Exists(x => x.Email.Equals(admin.Email)))
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
                    string token = Guid.NewGuid().ToString();
                    string url = string.Format("{0}{1}/{2}?value={3}", configuration["app_settings:WebEndpoint"], ValidationModule.ACTIVATE_ACCOUNT, token, "admin");
                    string filePath = System.IO.Path.Combine(environment.ContentRootPath, @"EmailTemplates\NewCustomer.html");
                    string html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                    html = html.Replace("{name}", admin.Email).Replace("{link}", HttpUtility.UrlEncode(url));

                    await distributedCache.SetStringAsync(token, admin.Email, expiryOptions);

                    EmailRequest emailRequest = new("Dominoes Society - Account Creation", html, admin.Email);
                    emailRequest.Settings = new ApplicationSetting { TestingMode = false, SettingName = "EmailNotification" };
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
            if (!admin.IsActive.Value)
            {
                response.Success = false;
                response.Message = $"<html>User not verified, <b><a href='{environment.WebRootPath}/customer/activate/{login.Email}'>click here</a></b> to resend verification link</html>";
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
            if (!string.IsNullOrEmpty(uniqueRef))
            {
                var admin = adminRepository.GetUser(uniqueRef);
                admin.IsActive = true;
                adminRepository.UpdateUser(admin);

                response.Message = $"User {uniqueRef} successfully activated, kindly login to access the application";
                response.Success = true;
                return response;
            }
            return response;
        }

        [HttpDelete("{email}")]
        [Authorize(Roles = "Admin", Policy = "Super")]
        public ApiResponse DeleteUser([EmailAddress(ErrorMessage = "Not a valid email address")] string email)
        {
            var admin = adminRepository.GetUser(email);
            if (admin != null && !admin.IsDeleted.Value)
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

        protected string GenerateJwtToken(string uniqueRef)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["app_settings:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.UniqueName, uniqueRef),
                new Claim(JwtRegisteredClaimNames.Jti, Convert.ToString(Guid.NewGuid()))
            };

            var token = new JwtSecurityToken(configuration["app_settings:Issuer"],
               configuration["app_settings:Issuer"], claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
