using System;
using System.Globalization;
using System.IO;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using DominoesProperties.Services;
using Helpers;
using Helpers.PayStack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private static readonly PaystackApiManager payStackApi = new();
        private readonly IConfiguration configuration;
        private readonly ICustomerRepository customerRepository;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private readonly IInvestmentRepository investmentRepository;
        private readonly ILoggerManager logger;
        private readonly IPaystackRepository paystackRepository;
        private readonly IPropertyRepository propertyRepository;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");
        private readonly ITransactionRepository transactionRepository;
        private readonly IWalletRepository walletRepository;

        public PaymentController(IConfiguration _configuration, ICustomerRepository _customerRepository,
            ILoggerManager _logger, IWebHostEnvironment _environment,
            IPaystackRepository _paystackRepository, ITransactionRepository _transactionRepository,
            IWalletRepository _walletRepository,
            IInvestmentRepository _investmentRepository, IEmailService _emailService,
            IPropertyRepository _propertyRepository)
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
            propertyRepository = _propertyRepository;
        }

        [HttpPost]
        [Authorize]
        public ApiResponse InitiateTransaction([FromBody] Payment payment)
        {
            return DoInitPayment(payment, HttpContext.User.Identity!.Name);
        }

        [HttpPost("init")]
        [Authorize]
        public ApiResponse DoInitPayment(Payment payment, string user)
        {
            var customer = customerRepository.GetCustomer(user);
            _ = decimal.TryParse(configuration["subscription"], out var subscription);
            var amount = payment.Module.Equals(PaymentType.SUBSCRIPTION) ? subscription : payment.Amount;
            var transRef = Guid.NewGuid().ToString();
            PaymentModel m = new()
            {
                Amount = AddPaystackCharges(amount) * 100,
                Email = customer.Email,
                Reference = string.IsNullOrEmpty(payment.InvestmentId) ? transRef : payment.InvestmentId,
                Bearer = "subaccount",
                Callback = string.IsNullOrEmpty(payment.Callback)
                    ? $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/payment/verify-payment"
                    : $"{payment.Callback}"
            };
            
            logger.LogInfo($"Transaction initialized payload \n {Convert.ToString(m)}");
            try
            {
                var initResponse = payStackApi.MobileAppInitTransaction(m).Data;
                if (initResponse == null)
                    throw new InvalidOperationException("Payment platform error, please try again later");

                logger.LogInfo($"Transaction initialization completed with payload \n {initResponse}");

                var jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(initResponse));
                if (jObject == null)
                    throw new InvalidOperationException("Payment platform error, please try again later");

                PaystackPayment paystack = new()
                {
                    Payload = initResponse.ToString(),
                    AccessCode = Convert.ToString(jObject["access_code"]);
                    Amount = m.Amount,
                    TransactionRef = m.Reference,
                    PaystackRef = Convert.ToString(jObject["reference"]),
                    PaymentModule = payment.Module.ToString(),
                    Type = TransactionType.CR.ToString(),
                    Date = DateTime.Now
                };
                paystackRepository.NewPayment(paystack);
                response.Success = true;
                response.Message = "Successfully initialized payment link";
                response.Data = Convert.ToString(jObject["authorization_url"]);
                return response;
            }
            catch (Exception e)
            {
                if (payment.Module.Equals(PaymentType.PROPERTY_PAIRING_GROUP))
                {
                    var group = investmentRepository.GetSharinggroups(payment.InvestmentId);
                    investmentRepository.DeleteGroup(group);
                }

                logger.LogDebug(e.StackTrace);
                response.Success = false;
                response.Message = "Error initiating transaction status, we will re-confirm and get back to you";
                return response;
            }
        }

        [HttpGet("verify-payment")]
        public RedirectResult Subscribe([FromQuery] string reference)
        {
            try
            {
                var returns = Convert.ToString(payStackApi.VerifyTransaction(reference).Data);

                if (string.IsNullOrWhiteSpace(returns))
                {
                    logger.LogError(
                        $"{DateTime.Now} : Payment : verify-payment: {reference} | Unsuccessful transaction for reference");
                    return Redirect($"{configuration["app_settings:WebEndpoint"]}?reference={reference}status=error");
                }

                logger.LogInfo($"Paystack callback \n {returns}");
                var jObject = JsonConvert.DeserializeObject<JObject>(returns);
                if (jObject == null)
                    throw new InvalidOperationException("Payment platform error, please try again later");
                var cust = JsonConvert.DeserializeObject<JObject>(jObject["customer"]!.ToString());

                var paystack = paystackRepository.GetPaystack(reference);
                paystack.Channel = Convert.ToString(jObject["channel"]);
                paystack.Status = Convert.ToString(jObject["status"]);
                paystack.Payload = CommonLogic.Encrypt(returns);
                paystack.Date = Convert.ToDateTime(jObject["transaction_date"]);

                paystackRepository.UpdatePayment(paystack);
                logger.LogInfo("Paystack table updated successfully");

                Transaction transaction = new()
                {
                    Amount = Convert.ToDecimal(jObject["amount"]) / 100,
                    Channel = paystack.Channel
                };
                var customer = customerRepository.GetCustomer(Convert.ToString(cust["email"]));
                transaction.CustomerId = customer.Id;
                transaction.Module = paystack.PaymentModule;
                transaction.Status = paystack.Status;
                transaction.TransactionRef = paystack.PaymentModule.Equals(PaymentType.PROPERTY_PAIRING.ToString())
                    ? $"{paystack.TransactionRef}-{new Random().Next(2, 10)}"
                    : paystack.TransactionRef;
                transaction.TransactionType = TransactionType.CR.ToString();
                transaction.TransactionDate = DateTime.Now;

                transactionRepository.NewTransaction(transaction);
                logger.LogInfo("Transaction table updated successfully");

                if (paystack.PaymentModule.Equals(PaymentType.FUND_WALLET.ToString()))
                {
                    var wallet = walletRepository.GetCustomerWallet(transaction.CustomerId);
                    wallet.Balance += paystack.Amount;
                    wallet.LastTransactionAmount = paystack.Amount;
                    wallet.LastTransactionDate = DateTime.Now;
                    walletRepository.UpdateCustomerWallet(wallet);

                    SendMail(customer.Email, customer.FirstName, customer.LastName, "wallet.html",
                        "Your wallet has been successfully funded", paystack.Amount);
                }
                else if (paystack.PaymentModule.Equals(PaymentType.PROPERTY_PAIRING_GROUP.ToString()))
                {
                    var group = investmentRepository.GetSharinggroups(paystack.TransactionRef);
                    if (!paystack.Status!.ToLower().Equals("success"))
                    {
                        investmentRepository.DeleteGroup(group);
                        response.Message = "Payment was not successful, group was not created";

                        logger.LogInfo($"{transaction.TransactionRef} : {reference} : {paystack.Status}");
                        logger.LogInfo($"{paystack.TransactionRef} : Payment unsuccessfull");
                        return Redirect($"{configuration["app_settings:WebEndpoint"]}?status={paystack.Status}");
                    }

                    var property = propertyRepository.GetProperty(group.PropertyId);
                    Sharingentry entry = new()
                    {
                        CustomerId = customer.Id,
                        GroupId = group.UniqueId,
                        PercentageShare = decimal.ToInt32(decimal.Round(
                            decimal.Divide(transaction.Amount, property.UnitPrice) * 100, MidpointRounding.ToZero)),
                        Date = DateTime.Now,
                        IsClosed = false
                    };
                    if (investmentRepository.AddSharingentry(entry))
                    {
                        group.PercentageSubscribed += entry.PercentageShare;
                        investmentRepository.UpdateSharingGroup(group);
                        logger.LogInfo($"{transaction.TransactionRef} : {reference} : {paystack.Status}");
                        logger.LogInfo($"{paystack.TransactionRef} : Payment successfully done");
                        return Redirect($"{configuration["app_settings:WebEndpoint"]}?status=success");
                    }
                }
                else if (paystack.PaymentModule.Equals(PaymentType.PROPERTY_PAIRING.ToString()))
                {
                    var spp = transaction.TransactionRef[
                        ..transaction.TransactionRef.LastIndexOf("-", StringComparison.Ordinal)];
                    var group = investmentRepository.GetSharinggroups(spp);
                    var property = propertyRepository.GetProperty(group.PropertyId);

                    var cc = decimal.ToInt32(decimal.Round(decimal.Divide(transaction.Amount, property.UnitPrice) * 100,
                        MidpointRounding.ToZero));
                    var xx = cc * 100;
                    var yy = decimal.Round(xx, MidpointRounding.ToZero);
                    var ff = decimal.ToInt32(yy);

                    Sharingentry entry = new()
                    {
                        CustomerId = customer.Id,
                        GroupId = group.UniqueId,
                        PercentageShare = decimal.ToInt32(decimal.Round(
                            decimal.Divide(transaction.Amount, property.UnitPrice) * 100, MidpointRounding.ToZero)),
                        Date = DateTime.Now,
                        IsClosed = false,
                        PaymentReference = transaction.TransactionRef
                    };
                    if (investmentRepository.AddSharingentry(entry))
                    {
                        group.PercentageSubscribed += entry.PercentageShare;
                        investmentRepository.UpdateSharingGroup(group);

                        logger.LogInfo($"{transaction.TransactionRef} : {reference} : {paystack.Status}");
                        logger.LogInfo($"{paystack.TransactionRef} : Payment successfully done");
                        return Redirect($"{configuration["app_settings:WebEndpoint"]}?status=success");
                    }
                }
                else if (paystack.PaymentModule.Equals(PaymentType.PROPERTY_PURCHASE.ToString()))
                {
                    var investment = investmentRepository.GetNewInvestments(paystack.TransactionRef);
                    investment.Status = paystack.Status.ToLower().Equals("success") ? "COMPLETED" : "DECLINED";
                    investment.Property.UnitAvailable = investment.Property.UnitAvailable - investment.Units;
                    investment.Property.UnitSold = investment.Property.UnitSold + investment.Units;

                    string filePath = "", html = "";
                    EmailData emailData;
                    if (investment.Property.UnitAvailable == 0)
                    {
                        investment.Property.Status = PropertyStatus.CLOSED_FOR_INVESTMENT.ToString();

                        filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\investment-update.html");
                        html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                        html = html.Replace("{I-NAME}", investment.Property.Name)
                            .Replace("{webroot}", configuration["app_settings:WebEndpoint"]);
                        ;

                        emailData = new EmailData
                        {
                            EmailBody = html,
                            EmailSubject = $"{investment.Property.Name}  is fully subscribed. Thank you!",
                            EmailToId = customer.Email,
                            EmailToName = customer.FirstName
                        };
                        //TODO send broadcast
                    }

                    investmentRepository.UpdateInvestment(investment);

                    filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\investment.html");
                    html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                    html = html.Replace("{FIRSTNAME}", string.Format("{0} {1}", customer.FirstName, customer.LastName))
                        .Replace("{I-NAME}", investment.Property.Name);
                    html = html.Replace("{I-UNITS}", investment.Units.ToString())
                        .Replace("{I-PRICE}", investment.Property.UnitPrice.ToString())
                        .Replace("{I-TOTAL}", paystack.Amount.ToString())
                        .Replace("{I-DATE}", investment.PaymentDate.ToString())
                        .Replace("{webroot}", configuration["app_settings:WebEndpoint"]);
                    ;

                    emailData = new EmailData
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
                    customer.IsSubscribed = true;
                    if (customer.NextSubscriptionDate.HasValue)
                        customer.PrevSubscriptionDate = customer.NextSubscriptionDate;
                    customer.NextSubscriptionDate = DateTime.Now.Date.AddYears(1);
                    customer.IsVerified = true;
                    customer.IsActive = true;
                    customerRepository.UpdateCustomer(customer);
                    SendMail(customer.Email, customer.FirstName, customer.LastName, "subscription.html",
                        "Congratulations!, You are in, and have access to co-invest with us");
                }

                logger.LogInfo($"{transaction.TransactionRef} : {reference} : {paystack.Status}");
                logger.LogInfo($"{paystack.TransactionRef} : Payment successfully done");
                return Redirect($"{configuration["app_settings:WebEndpoint"]}?status={paystack.Status}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                logger.LogError(
                    $"{DateTime.Now} : Payment : verify-payment: {reference} | Error verifying transaction status");
                return Redirect($"{configuration["app_settings:WebEndpoint"]}?reference={reference}&status=error");
            }
        }

        private void SendMail(string email, string firstName, string lastName, string filename, string subject,
            decimal sum = 0)
        {
            var filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\" + filename);
            var html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
            html = html.Replace("{FIRSTNAME}", string.Format("{0} {1}", firstName, lastName))
                .Replace("{SUM}", sum.ToString()).Replace("{webroot}", configuration["app_settings:WebEndpoint"]);

            EmailData emailData = new()
            {
                EmailBody = html,
                EmailSubject = subject,
                EmailToId = email,
                EmailToName = firstName
            };
            emailService.SendEmail(emailData);
        }

        private static decimal AddPaystackCharges(decimal amount)
        {
            decimal fee;
            if (amount < 2500)
                fee = amount * Convert.ToDecimal(1.5 / 100);
            else
                fee = amount * Convert.ToDecimal(1.5 / 100) + 100;

            return amount + (fee > 2000 ? 2000 : fee);
        }
    }
}