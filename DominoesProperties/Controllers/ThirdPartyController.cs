using DominoesProperties.Helper;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThirdPartyController
    {
        private readonly IThirdPartyClientRepository _clientRepository;
        private readonly CustomerController _customerController;
        private readonly ICustomerRepository _customerRepository;
        private readonly ApiResponse _response = new(false, "Error performing request, contact admin");

        public ThirdPartyController(ICustomerRepository customerRepository, CustomerController customerController,
            IThirdPartyClientRepository clientRepository)
        {
            _customerRepository = customerRepository;
            _customerController = customerController;
            _clientRepository = clientRepository;
        }

        [HttpPost("affiliate-customer")]
        public ApiResponse ThirdPartySignUp([FromBody] AffiliateCustomer customer, [FromHeader] int clientId,
            [FromHeader] string apiKey)
        {
            if (clientId == 100)
            {
                _response.Success = false;
                _response.Message =
                    $"Forbidden: This client id is not permitted to create a third party customer";
                return _response;
            }

            var client = _clientRepository.GetClient(clientId, apiKey);
            if (client == null)
            {
                _response.Success = false;
                _response.Message =
                    $"Unauthorized access: Invalid API key supplied";
                return _response;
            }

            var customerReq = ClassConverter.ThirdPartyCustomerToCustomer(customer);
            var tt = _customerRepository.CreateThirdPartyCustomer(customer.Email, customer.Phone, customer.LastName,
                customer.FirstName, client.ClientId);
            if (tt != null) return _customerController.RegisterAsync(customerReq);

            _response.Success = false;
            _response.Message =
                $"Error creating customer on dominoes platform, existing user {customer.Email} or {customer.Phone}";
            return _response;
        }
    }
}