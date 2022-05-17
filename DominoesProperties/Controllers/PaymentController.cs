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
using Microsoft.AspNetCore.Authorization;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private static readonly PaystackApiManager payStackApi = new();
        private readonly IConfiguration configuration;
        private readonly ICustomerRepository customerRepository;
        private readonly IStringLocalizer<PaymentController> localizer;
        private readonly IPaystackRepository paystackRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IWalletRepository walletRepository;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");
        
        public PaymentController(IConfiguration _configuration, ICustomerRepository _customerRepository, IStringLocalizer<PaymentController> _localizer,
            IPaystackRepository _paystackRepository, ITransactionRepository _transactionRepository, IWalletRepository _walletRepository)
        {
            configuration = _configuration;
            customerRepository = _customerRepository;
            localizer = _localizer;
            paystackRepository = _paystackRepository;
            transactionRepository = _transactionRepository;
            walletRepository = _walletRepository;
        }

        [HttpGet]
        [Authorize]
        public ApiResponse InitiateTransaction([FromBody] Payment payment)
        {
            var customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
            _ = decimal.TryParse(configuration["subscription"], out decimal subscription);
            var amount = payment.Module.Equals(PaymentType.SUBSCRIPTION) ? subscription : payment.Amount;
            var initResponse = payStackApi.MobileAppInitTransaction(
                    amount,
                    customer.Email,
                    string.Format("{0}://{1}/{2}/{3}", Request.Scheme, Request.Host, "verify-payment", "")
                ).Data;

            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(initResponse));
            PaystackPayment paystack = new();
            paystack.AccessCode = Convert.ToString(jObject["access_code"]);
            paystack.Amount = amount;
            paystack.TransactionRef = Convert.ToString(jObject["reference"]);
            paystack.PaymentModule = payment.Module.ToString();
            paystack.Type = TransactionType.CR.ToString();
            paystackRepository.NewPayment(paystack);
            response.Success = true;
            response.Message = localizer["Response.Success"];
            response.Data = Convert.ToString(jObject["authorization_url"]);
            return response;
        }

        [HttpGet("verify-payment/{reference}")]
        [Authorize]
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
            transaction.TransactionType = TransactionType.CR.ToString();

            transactionRepository.NewTransaction(transaction);

            if (paystack.PaymentModule.Equals(PaymentType.FUND_WALLET))
            {
                Wallet wallet = walletRepository.GetCustomerWallet(transaction.CustomerId);
                wallet.Balance = wallet.Balance + paystack.Amount;
                wallet.LastTransactionAmount = paystack.Amount;
                wallet.LastTransactionDate = DateTime.Now;
                walletRepository.UpdateCustomerWallet(wallet);
            }
        }
    }
}