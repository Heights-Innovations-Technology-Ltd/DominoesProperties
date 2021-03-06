using System;
using DominoesProperties.Models;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repositories.Repository;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Models.Models;
using DominoesProperties.Enums;
using Microsoft.AspNetCore.Authorization;
using Helpers.PayStack;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private static readonly PaystackApiManager payStackApi = new();
        private readonly IConfiguration configuration;
        private readonly ICustomerRepository customerRepository;
        private readonly IPaystackRepository paystackRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IInvestmentRepository investmentRepository;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");
        
        public PaymentController(IConfiguration _configuration, ICustomerRepository _customerRepository,
            IPaystackRepository _paystackRepository, ITransactionRepository _transactionRepository, IWalletRepository _walletRepository, IInvestmentRepository _investmentRepository)
        {
            configuration = _configuration;
            customerRepository = _customerRepository;
            paystackRepository = _paystackRepository;
            transactionRepository = _transactionRepository;
            walletRepository = _walletRepository;
            investmentRepository = _investmentRepository;
        }

        [HttpPost]
        [Authorize]
        public ApiResponse InitiateTransaction([FromBody] Payment payment)
        {
            return DoInitPayment(payment, HttpContext.User.Identity.Name);
        }

        [HttpPost("init")]
        [Authorize]
        public ApiResponse DoInitPayment(Payment payment, string user)
        {
            var customer = customerRepository.GetCustomer(user);
            _ = decimal.TryParse(configuration["subscription"], out decimal subscription);
            var amount = payment.Module.Equals(PaymentType.SUBSCRIPTION) ? subscription : payment.Amount;
            var transRef = Guid.NewGuid().ToString();
            PaymentModel m = new()
            {
                amount = amount * 100,
                email = customer.Email,
                reference = transRef,
                callback = string.IsNullOrEmpty(payment.Callback)
                ? string.Format("{0}/{1}/{2}", $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}", "api/payment/verify-payment", transRef)
                : $"{payment.Callback}/{transRef}"
            };

            var initResponse = payStackApi.MobileAppInitTransaction(m).Data;
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(initResponse));
            PaystackPayment paystack = new();
            paystack.AccessCode = Convert.ToString(jObject["access_code"]);
            paystack.Amount = amount;
            paystack.TransactionRef = transRef;
            paystack.PaymentModule = payment.Module.ToString();
            paystack.Type = TransactionType.CR.ToString();
            paystackRepository.NewPayment(paystack);
            response.Success = true;
            response.Message = "Successfully initialized payment link";
            response.Data = Convert.ToString(jObject["authorization_url"]);
            return response;
        }

        [HttpGet("verify-payment/{reference}")]
        public ApiResponse Subscribe(string reference)
        {
            var returns = Convert.ToString(payStackApi.VerifyTransaction(reference).Data);
            if (string.IsNullOrWhiteSpace(returns))
            {
                response.Message = "Unsuccessful transaction, try again later";
                return response;
            }

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
            transaction.TransactionType = TransactionType.CR.ToString();

            transactionRepository.NewTransaction(transaction);

            if (paystack.PaymentModule.Equals(PaymentType.FUND_WALLET))
            {
                Wallet wallet = walletRepository.GetCustomerWallet(transaction.CustomerId);
                wallet.Balance = wallet.Balance + paystack.Amount;
                wallet.LastTransactionAmount = paystack.Amount;
                wallet.LastTransactionDate = DateTime.Now;
                walletRepository.UpdateCustomerWallet(wallet);
            }else if(paystack.PaymentModule.Equals(PaymentType.PROPERTY_PURCHASE)){
                // var investment = investmentRepository.GetInvestments().Where(x => x.TransactionRef.Equals(paystack.TransactionRef));
            }

            response.Success = true;
            response.Message = string.Format("Payment successfully done");
            return response;
        }
    }
}