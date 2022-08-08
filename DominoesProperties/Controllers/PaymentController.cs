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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using DominoesProperties.Services;

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
        private readonly ILoggerManager logger;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");

        public PaymentController(IConfiguration _configuration, ICustomerRepository _customerRepository, ILoggerManager _logger, IWebHostEnvironment _environment,
            IPaystackRepository _paystackRepository, ITransactionRepository _transactionRepository, IWalletRepository _walletRepository,
            IInvestmentRepository _investmentRepository, IEmailService _emailService)
        {
            configuration = _configuration;
            customerRepository = _customerRepository;
            paystackRepository = _paystackRepository;
            transactionRepository = _transactionRepository;
            walletRepository = _walletRepository;
            investmentRepository = _investmentRepository;
            logger = _logger;
            environment = _environment;
            emailService = _emailService;
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
                reference = string.IsNullOrEmpty(payment.InvestmentId) ? transRef : payment.InvestmentId,
                callback = string.IsNullOrEmpty(payment.Callback)
                ? string.Format("{0}/{1}", $"{Request.Scheme}://{Request.Host}{Request.PathBase}", "api/payment/verify-payment")
                : $"{payment.Callback}"
            };

            var initResponse = payStackApi.MobileAppInitTransaction(m).Data;
            try
            {
                JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(initResponse));
                PaystackPayment paystack = new();
                paystack.AccessCode = Convert.ToString(jObject["access_code"]);
                paystack.Amount = amount;
                paystack.TransactionRef = m.reference;
                paystack.PaystackRef = Convert.ToString(jObject["reference"]);
                paystack.PaymentModule = payment.Module.ToString();
                paystack.Type = TransactionType.CR.ToString();
                paystackRepository.NewPayment(paystack);
                response.Success = true;
                response.Message = "Successfully initialized payment link";
                response.Data = Convert.ToString(jObject["authorization_url"]);
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.StackTrace);
                response.Message = "Error verifying transaction status, we will re-confirm and get back to you";
                return response;
            }
        }

        [HttpGet("verify-payment")]
        public RedirectResult Subscribe([FromQuery] string reference)
        {
            var returns = Convert.ToString(payStackApi.VerifyTransaction(reference).Data);
            if (string.IsNullOrWhiteSpace(returns))
            {
                logger.LogError($"{DateTime.Now} : Payment : verify-payment: {reference} | Unsuccessful transaction for reference");
                return Redirect($"{configuration["app_settings:WebEndpoint"]}?reference={reference}status=error");
            }

            JObject jObject = JsonConvert.DeserializeObject<JObject>(returns);
            JObject cust = JsonConvert.DeserializeObject<JObject>(jObject["customer"].ToString());
            try
            {
                PaystackPayment paystack = paystackRepository.GetPaystack(reference);
                paystack.Channel = Convert.ToString(jObject["channel"]);
                paystack.Status = Convert.ToString(jObject["status"]);
                paystack.Payload = CommonLogic.Encrypt(returns);
                paystack.Date = Convert.ToDateTime(jObject["transaction_date"]);

                paystackRepository.UpdatePayment(paystack);

                Transaction transaction = new();
                transaction.Amount = Convert.ToDecimal(jObject["amount"]);
                transaction.Channel = paystack.Channel;
                var customer = customerRepository.GetCustomer(Convert.ToString(cust["email"]));
                transaction.CustomerId = customer.Id;
                transaction.Module = paystack.PaymentModule;
                transaction.Status = paystack.Status;
                transaction.TransactionRef = paystack.TransactionRef;
                transaction.TransactionType = TransactionType.CR.ToString();

                transactionRepository.NewTransaction(transaction);

                if (paystack.PaymentModule.Equals(PaymentType.FUND_WALLET.ToString()))
                {
                    Wallet wallet = walletRepository.GetCustomerWallet(transaction.CustomerId);
                    wallet.Balance += paystack.Amount;
                    wallet.LastTransactionAmount = paystack.Amount;
                    wallet.LastTransactionDate = DateTime.Now;
                    walletRepository.UpdateCustomerWallet(wallet);

                    sendMail(customer.Email, customer.FirstName, customer.LastName, "wallet.html", "Your wallet has been successfully funded", paystack.Amount);
                }
                else if (paystack.PaymentModule.Equals(PaymentType.PROPERTY_PURCHASE.ToString()))
                {
                    var investment = investmentRepository.GetNewInvestments(paystack.TransactionRef);
                    investment.Status = paystack.Status.ToLower().Equals("success") ? "COMPLETED" : "DECLINED";
                    investment.Property.UnitAvailable = investment.Property.UnitAvailable - investment.Units;
                    investment.Property.UnitSold = investment.Property.UnitSold + investment.Units;

                    string filePath = "", html = "";
                    EmailData emailData;
                    if(investment.Property.UnitAvailable == 0)
                    {
                        investment.Property.Status = PropertyStatus.CLOSED_FOR_INVESTMENT.ToString();

                        filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\investment-update.html");
                        html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                        html = html.Replace("{I-NAME}", investment.Property.Name);

                        emailData = new()
                        {
                            EmailBody = html,
                            EmailSubject = $"{investment.Property.Name}  is fully subscribed. Thank you!",
                            EmailToId = customer.Email,
                            EmailToName = customer.FirstName
                        };
                    }
                    
                    investmentRepository.UpdateInvestment(investment);

                    filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\investment.html");
                    html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                    html = html.Replace("{FIRSTNAME}", string.Format("{0} {1}", customer.FirstName, customer.LastName)).Replace("{I-NAME}", investment.Property.Name);
                    html = html.Replace("{I-UNITS}", investment.Units.ToString()).Replace("{I-PRICE}", investment.Property.UnitPrice.ToString()).Replace("{I-TOTAL}", paystack.Amount.ToString()).Replace("{I-DATE}", investment.PaymentDate.ToString());

                    emailData = new()
                    {
                        EmailBody = html,
                        EmailSubject = "Congratulations!!! You just made an investment",
                        EmailToId = customer.Email,
                        EmailToName = customer.FirstName
                    };
                    emailService.SendEmail(emailData);
                }
                else if (paystack.PaymentModule.Equals(PaymentType.SUBSCRIPTION.ToString()))
                {
                    sendMail(customer.Email, customer.FirstName, customer.LastName, "subscription.html", "Congratulations!, You are in and have access to co-invest with us");
                }

                logger.LogError($"{transaction.TransactionRef} : {reference} : {paystack.Status}");
                logger.LogError($"{paystack.TransactionRef} : Payment successfully done");
                return Redirect($"{configuration["app_settings:WebEndpoint"]}?status={paystack.Status}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                logger.LogError($"{DateTime.Now} : Payment : verify-payment: {reference} | rror verifying transaction status");
                return Redirect($"{configuration["app_settings:WebEndpoint"]}?reference={reference}status=error");
            }
        }

        private void sendMail(string Email, string FirstName, string LastName, string Filename, string Subject, decimal sum = 0)
        {
            string filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\"+Filename);
            string html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
            html = html.Replace("{FIRSTNAME}", string.Format("{0} {1}", FirstName, LastName)).Replace("{SUM}", sum.ToString());

            EmailData emailData = new()
            {
                EmailBody = html,
                EmailSubject = Subject,
                EmailToId = Email,
                EmailToName = FirstName
            };
            emailService.SendEmail(emailData);
        }
    }
}