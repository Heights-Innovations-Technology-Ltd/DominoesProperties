using System;
using DominoesProperties.Enums;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
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
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ApiResponse _response = new(false, "Error performing request, contact admin");

        public ThirdPartyController(ICustomerRepository customerRepository, CustomerController customerController,
            IThirdPartyClientRepository clientRepository, IInvestmentRepository investmentRepository,
            IPropertyRepository propertyRepository)
        {
            _customerRepository = customerRepository;
            _customerController = customerController;
            _clientRepository = clientRepository;
            _investmentRepository = investmentRepository;
            _propertyRepository = propertyRepository;
        }

        [HttpPost("affiliate-investment")]
        public ApiResponse ThirdPartyPurchase([FromBody] AffiliateCustomer customer, [FromHeader] int clientId,
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

            var property = _propertyRepository.GetProperty(customer.PropertyId);
            if (property == null || property.UnitPrice != (customer.Amount / customer.Units))
            {
                _response.Success = false;
                _response.Message = property == null
                    ? $"Unknown property with unique reference {customer.PropertyId} not found or invalid price supplied."
                    : $" Property price ins {property!.UnitPrice}";
                return _response;
            }

            var refKey = _investmentRepository.GetThirdPartyInvestment(customer.TransactionRef);
            if (refKey != null)
            {
                _response.Success = false;
                _response.Message =
                    $"Error creating investment for transaction, transction reference {customer.TransactionRef} already exists";
                return _response;
            }

            var customerReq = ClassConverter.ThirdPartyCustomerToCustomer(customer);
            var tt =
                _customerRepository.GetThirdPartyCustomer(customer.Email == null ? customer.Email : customer.Phone) ??
                _customerRepository.CreateThirdPartyCustomer(customer.Email, customer.Phone, customer.LastName,
                    customer.FirstName, client.ClientId);

            if (tt == null)
            {
                _response.Success = false;
                _response.Message =
                    $"Error creating investment for third party customer {customer.FirstName} {customer.LastName}";
                return _response;
            }

            var tf = _customerRepository.GetCustomer(customer.Email == null ? customer.Email : customer.Phone);
            long customerId;
            if (tf == null)
            {
                var cust = _customerController.RegisterAsync(customerReq, true);
                if (!cust.Success)
                {
                    _response.Success = false;
                    _response.Message =
                        $"Error creating investment for customer {customer.FirstName} {customer.LastName}";
                    return _response;
                }

                customerId = (long)cust.Data;
            }
            else
                customerId = tf.Id;

            var thirdPartyInvestment = new Thirdpartyinvestment
            {
                Amount = customer.Amount,
                Units = customer.Units,
                CustomerId = customerId,
                PaymentDate = DateTime.Now,
                PaymentType = PaymentType.SUBSCRIPTION.ToString(),
                UnitPrice = property.UnitPrice,
                TransactionRef = customer.TransactionRef,
                PropertyId = property.Id
            };
            var r = _investmentRepository.AddThirdPartyInvestment(thirdPartyInvestment);
            if (r > 0)
            {
                property.UnitSold += customer.Units;
                property.UnitAvailable -= customer.Units;
                _propertyRepository.UpdateProperty(property);

                _response.Success = true;
                _response.Message =
                    $"User created and investment has been successfully added";
                return _response;
            }

            _response.Success = false;
            _response.Message =
                $"Error creating customer on dominoes platform, existing user {customer.Email} or {customer.Phone}";
            return _response;
        }
    }
}