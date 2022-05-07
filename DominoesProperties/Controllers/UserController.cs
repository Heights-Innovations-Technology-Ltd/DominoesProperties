using DominoesProperties.Helper;
using DominoesProperties.Localize;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Repositories.Repository;
using System.Net;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> logger;
        private readonly ICustomerRepository customerRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IStringLocalizer<Resource> localizer;
        private ApiResponse response = new ApiResponse(HttpStatusCode.BadRequest, "Error performing request, contact admin");

        public UserController(ILogger<UserController> _logger, ICustomerRepository _customerRepository, IStringLocalizer<Resource> _stringLocalizer,
            IWalletRepository _walletRepository)
        {
            logger = _logger;
            customerRepository = _customerRepository;
            localizer = _stringLocalizer;
            walletRepository = _walletRepository;
        }

        [HttpPost]
        [Route("register")]
        public ApiResponse Register([FromBody] Models.Customer customer)
        {
            if (!customer.Password.Equals(customer.ConfirmPassword))
            {
                return new ApiResponse(HttpStatusCode.BadRequest, "Password does not match");
            }

            if (customerRepository.CreateCustomer(ClassConverter.ConvertCustomerToEntity(customer)))
            {
                //TODO send registration email to customer
                response.Code = HttpStatusCode.Created;
                response.Message = localizer["201"]; //"Customer registered successfully";
                logger.LogInformation(response.Message);
                return response;
            }
            return response;
        }

        [HttpPost]
        [Route("login")]
        public ApiResponse Login([FromBody] Login login)
        {
            var customer = customerRepository.GetCustomer(login.Email);
            if(customer == null)
            {
                response.Code = HttpStatusCode.BadRequest;
                response.Message = "Invalid username supplied";
                return response;
            }

            if (customer.Password.Equals(CommonLogic.Encrypt(customer.Password))){
                response.Data = ClassConverter.ConvertCustomerToProfile(customer);
                response.Code = HttpStatusCode.OK;
                response.Message = response.Message = localizer["200"];
                return response;
            }
            else
            {
                response.Code = HttpStatusCode.BadRequest;
                response.Message = "Invalid password supplied";
                return response;
            }
        }
    }
}
