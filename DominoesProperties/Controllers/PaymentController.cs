using System;
using DominoesProperties.Models;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repositories.Repository;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Models.Models;
using DominoesProperties.Enums;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly PaystackApiManager payStackApi = new();
        private readonly IConfiguration configuration;
        private readonly ICustomerRepository customerRepository;
        private readonly IStringLocalizer<PaymentController> localizer;
        private readonly IPaystackRepository paystackRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly ApiResponse response = new ApiResponse(false, "Error performing request, contact admin");
        
        public PaymentController(IConfiguration _configuration, ICustomerRepository _customerRepository, IStringLocalizer<PaymentController> _localizer,
            IPaystackRepository _paystackRepository, ITransactionRepository _transactionRepository)
        {
            configuration = _configuration;
            customerRepository = _customerRepository;
            localizer = _localizer;
            paystackRepository = _paystackRepository;
            transactionRepository = _transactionRepository;
        }

        [HttpGet("{uniqueId}/{module}")]
        public IActionResult InitiateTransaction(string uniqueId, PaymentType module)
        {
            var customer = customerRepository.GetCustomer(uniqueId);
            _ = decimal.TryParse(configuration["subscription"], out decimal subscription);

            var initResponse = payStackApi.MobileAppInitTransaction(
                    subscription,
                    customer.Email,
                    string.Format("{0}://{1}/{2}/{3}", Request.Scheme, Request.Host, "verify-payment", "")
                ).Data;
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(initResponse));
            PaystackPayment paystack = new();
            paystack.AccessCode = Convert.ToString(jObject["access_code"]);
            paystack.Amount = subscription;
            paystack.TransactionRef = Convert.ToString(jObject["reference"]);
            paystack.PaymentModule = module.ToString();
            paystackRepository.NewPayment(paystack);
            return Redirect(Convert.ToString(jObject["authorization_url"]));
        }

        [HttpGet("verify-payment/{reference}")]
        public void Subscribe(string reference)
        {
            var returns = Convert.ToString(payStackApi.VerifyTransaction(reference).Data);
            JObject jObject = JsonConvert.DeserializeObject<JObject>(returns);

            PaystackPayment paystack = paystackRepository.GetPaystack(reference);
            paystack.Channel = Convert.ToString(jObject["channel"]);
            paystack.Status = Convert.ToString(jObject["status"]);
            paystack.Payload = returns;

            Transaction transaction = new();
            transaction.Amount = Convert.ToDecimal(jObject["amount"]);
            transaction.Channel = paystack.Channel;
            transaction.CustomerId = customerRepository.GetCustomer(Convert.ToString(jObject["customer:email"])).Id;
            transaction.Module = paystack.PaymentModule;
            transaction.Status = paystack.Status;
            transaction.TransactionRef = paystack.TransactionRef;
            transaction.TransactionType = TransactionType.DEBIT.ToString();

            transactionRepository.NewTransaction(transaction);

        }

        [HttpGet("wallet/{uniqueId}")]
        public ApiResponse FundWallet(string uniqueId)
        {
            var customer = customerRepository.GetCustomer(uniqueId);
            if(customer == null){
                response.Message = localizer["Username.Error"];
                return response;
            }
            decimal subscription = new Decimal(0.00);
            Decimal.TryParse(configuration["subscription"], out subscription);
            response.Success = true;
            response.Message = localizer["Response.Success"];
            response.Data = payStackApi.MobileAppInitTransaction(subscription, customer.Email, "").Data;
            return response;
        }
    }
}
