using DominoesProperties.Helper;
using DominoesProperties.Localize;
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
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ILoggerManager logger;
        private readonly ICustomerRepository customerRepository;
        private readonly IStringLocalizer<Resource> localizer;
        private readonly IDistributedCache distributedCache;
        private readonly IConfiguration configuration;
        private ApiResponse response = new ApiResponse(HttpStatusCode.BadRequest, "Error performing request, contact admin");

        public UserController(ILoggerManager _logger, ICustomerRepository _customerRepository, IStringLocalizer<Resource> _stringLocalizer,
            IDistributedCache _distributedCache, IConfiguration _configuration)
        {
            logger = _logger;
            customerRepository = _customerRepository;
            localizer = _stringLocalizer;
            distributedCache = _distributedCache;
            configuration = _configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ApiResponse> RegisterAsync([FromBody] Models.Customer customer)
        {
            if (!customer.Password.Equals(customer.ConfirmPassword))
            {
                return new ApiResponse(HttpStatusCode.BadRequest, "Password does not match");
            }

            if (customerRepository.CreateCustomer(ClassConverter.ConvertCustomerToEntity(customer)))
            {
                string token = CommonLogic.GetUniqueRefNumber("AUTH");
                Dictionary<string, object> authData = new();
                authData.Add("expire", DateTime.Now);
                authData.Add("user", customer.UniqueReference);
                authData.Add("token", token);

                var cachedAuth = JsonSerializer.Serialize(authData);
                await distributedCache.SetStringAsync(token, cachedAuth);

                string url = string.Format("{0}/activate/{1}", configuration["app_settings:WebEndpoint"], token);

                //TODO create Html template for registration with url embedded

                EmailRequest emailRequest = new EmailRequest(localizer["Customer.Registration.Subject"], "Click on the below link to activate your account", customer.Email);
                CommonLogic.SendEmail(emailRequest);

                response.Code = HttpStatusCode.Created;
                response.Message = localizer["201"].Value.Replace("{params}", "Customer");
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
            if (customer == null)
            {
                response.Code = HttpStatusCode.BadRequest;
                response.Message = localizer["Username.Error"];
                return response;
            }

            if (customer.Password.Equals(CommonLogic.Encrypt(customer.Password)))
            {
                response.Data = ClassConverter.ConvertCustomerToProfile(customer);
                response.Code = HttpStatusCode.OK;
                response.Message = response.Message = localizer["200"];
                Response.Headers.Add("access_token", GenerateJwtToken(customer.UniqueRef));
                return response;
            }
            else
            {
                response.Code = HttpStatusCode.BadRequest;
                response.Message = localizer["Password.Error"];
                return response;
            }
        }

        [HttpDelete("uniqueRef")]
        [Authorize]
        public ApiResponse Delete(string uniqueRef)
        {
            customerRepository.DeleteCustomer(uniqueRef);
            response.Code = HttpStatusCode.OK;
            response.Message = response.Message = localizer["200"];
            return response;
        }

        [HttpPut("uniqueRef")]
        [Authorize]
        public ApiResponse Update(string uniqueRef, [FromBody] Models.Customer customer)
        {
            var existingCustomer = customerRepository.GetCustomer(uniqueRef);
            if (existingCustomer == null)
            {
                response.Message = $"Customer with id {uniqueRef} not found.";
                return response;
            }

            existingCustomer.Address = customer.Address;
            existingCustomer.AccountNumber = customer.AccountNumber;
            existingCustomer.Phone = customer.Phone;

            response.Message = localizer["200"];
            response.Code = HttpStatusCode.OK;
            response.Data = customerRepository.UpdateCustomer(existingCustomer);
            return response;
        }

        [HttpPut]
        [Route("/activate/{token}")]
        public async Task<ApiResponse> Activate(string token)
        {
            var cachedAuth = await distributedCache.GetStringAsync(token);
            if (!string.IsNullOrEmpty(cachedAuth))
            {
                // loaded data from the redis cache.
                var auth = JsonSerializer.Deserialize<Dictionary<string, object>>(cachedAuth);
                DateTime expire = (DateTime)auth["expire"];
                if(DateTime.Now > expire.AddMinutes(30)){
                    response.Message = localizer["Auth.Token.Expired"];
                    return response;
                }

                var customer = customerRepository.GetCustomer(auth["user"].ToString());
                customer.IsActive = true;
                customer.IsVerified = true;
                customerRepository.UpdateCustomer(customer);

                response.Message = localizer["200"];
                response.Code = HttpStatusCode.OK;
                response.Data = ClassConverter.ConvertCustomerToProfile(customer);
                return response;
            }
            return response;
        }

        [HttpGet]
        [Route("/activate/{uniqueRef}")]
        public async Task<ApiResponse> SendActivationLink(string uniqueRef)
        {
            var customer = customerRepository.GetCustomer(uniqueRef);
            if(customer != null){
                string token = CommonLogic.GetUniqueRefNumber("AUTH");
                Dictionary<string, object> authData = new();
                authData.Add("expire", DateTime.Now);
                authData.Add("user", customer.UniqueRef);
                authData.Add("token", token);

                var cachedAuth = JsonSerializer.Serialize(authData);
                await distributedCache.SetStringAsync(token, cachedAuth);

                string url = string.Format("{0}/activate/{1}", configuration["app_settings:WebEndpoint"], token);

                //TODO create Html template for registration with url embedded

                EmailRequest emailRequest = new EmailRequest(localizer["Customer.Registration.Subject"], "Click on the below link to activate your account", customer.Email);
                CommonLogic.SendEmail(emailRequest);

                response.Message = localizer["Auth.Link.Generated"];
                response.Code = HttpStatusCode.OK;
                return response;
            }else{
                response.Message = localizer["Username.Error"];
                return response;
            }
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
