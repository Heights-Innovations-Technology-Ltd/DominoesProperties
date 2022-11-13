using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DominoesProperties.Enums;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using DominoesProperties.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Models;
using Newtonsoft.Json;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ICustomerRepository customerRepository;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment env;
        private readonly IInvestmentRepository investmentRepository;
        private readonly PaymentController paymentController;
        private readonly IPropertyRepository propertyRepository;
        private readonly ApiResponse response = new ApiResponse(false, "Error performing request, contact admin");
        private readonly IUploadRepository uploadRepository;

        public InvestmentController(IPropertyRepository _propertyRepository, IConfiguration _configuration,
            ICustomerRepository _customerRepository, IInvestmentRepository _investmentRepository,
            PaymentController _paymentController, IWebHostEnvironment _env,
            IEmailService _emailService, IUploadRepository _uploadRepository)
        {
            propertyRepository = _propertyRepository;
            customerRepository = _customerRepository;
            investmentRepository = _investmentRepository;
            paymentController = _paymentController;
            configuration = _configuration;
            env = _env;
            emailService = _emailService;
            uploadRepository = _uploadRepository;
        }

        [HttpGet("pair-groups/{propertyUniqueId}")]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse InvestmentGroup(string propertyUniqueId)
        {
            List<Sharinggroup> groups;
            var property = propertyRepository.GetProperty(propertyUniqueId);
            groups = investmentRepository.GetSharinggroups(property.Id);
            if (groups.Count > 0)
            {
                response.Success = true;
                response.Message = "Pairing groups successfully fetched";
            }
            else
            {
                response.Message = "No available groups for this investment";
            }

            response.Data = groups;
            return response;
        }

        [HttpPost("pair-groups")]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse AddInvestmentGroup([FromBody] SharingGroup investment)
        {
            var property = propertyRepository.GetProperty(investment.PropertyUniqueId);
            if (property == null)
            {
                response.Message = "Invalid parameter in request, check to confirm property identifier is valid";
                return response;
            }
            else if (!property.AllowSharing.Value)
            {
                response.Message = "Pairing is not allowed on this property";
                return response;
            }
            else if (property.UnitAvailable < 1)
            {
                response.Message = "Property is fully subscribed";
                return response;
            }

            Sharinggroup shg = new()
            {
                Alias = investment.Alias,
                CustomerUniqueId = HttpContext.User.Identity.Name,
                PropertyId = property.Id,
                Date = DateTime.Now,
                MaxCount = 100 / property.MinimumSharingPercentage.Value,
                UniqueId = CommonLogic.GetUniqueRefNumber("pg"),
                IsClosed = false,
                UnitPrice = property.UnitPrice
            };

            if (investmentRepository.AddSharingGroup(shg))
            {
                Payment pay = new()
                {
                    Amount = (property.UnitPrice * investment.PercentageShare) / 100,
                    Module = PaymentType.PROPERTY_PAIRING_GROUP,
                    InvestmentId = shg.UniqueId,
                    Callback = string.Format("{0}/{1}", $"{Request.Scheme}://{Request.Host}{Request.PathBase}",
                        "api/payment/verify-payment")
                };
                return paymentController.DoInitPayment(pay, HttpContext.User.Identity.Name);
            }

            response.Message =
                "Unable to create new group for this property, either pairing is not allow or property is fully subscribed";
            return response;
        }

        [HttpPost("pair-invest")]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse AddInvestmentShareEntry([FromBody] InvestmentSharing investment)
        {
            var property = propertyRepository.GetProperty(investment.PropertyUniqueId);
            if (property == null)
            {
                response.Message = $"Property with id {investment.PropertyUniqueId} not found";
                return response;
            }

            if (property.UnitAvailable < 1)
            {
                response.Message = $"Pair investment closed for this property";
                return response;
            }

            Payment pay = new()
            {
                Amount = (property.UnitPrice * investment.PercentageShare) / 100,
                Module = PaymentType.PROPERTY_PAIRING,
                InvestmentId = investment.SharingGroupId,
                Callback = string.Format("{0}/{1}", $"{Request.Scheme}://{Request.Host}{Request.PathBase}",
                    "api/payment/verify-payment")
            };
            return paymentController.DoInitPayment(pay, HttpContext.User.Identity.Name);
        }

        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse Investments([FromBody] InvestmentNew investment)
        {
            var property = propertyRepository.GetProperty(investment.PropertyUniqueId);
            if (property == null)
            {
                response.Message = $"Property with id {investment.PropertyUniqueId} not found";
                return response;
            }

            var customer = customerRepository.GetCustomer(HttpContext.User.Identity!.Name);
            if (investment.Units > property.UnitAvailable)
            {
                response.Message =
                    $"Not enough investment units available, only {property.UnitAvailable} units available for purchase";
                return response;
            }

            if (investment.Units > property.MaxUnitPerCustomer)
            {
                response.Message =
                    $"Maximum number of investment units of {property.MaxUnitPerCustomer} allowed per customer exceeded";
                return response;
            }

            var amount = property.UnitPrice * investment.Units;

            switch (investment.PaymentChannel)
            {
                case Channel.WALLET when customer.Wallet.Balance < amount:
                    response.Message =
                        "Low wallet balance, please fund your wallet or try a different payment method to complete your investment.";
                    return response;
                case Channel.WALLET:
                {
                    Investment newInvestment = new()
                    {
                        Amount = amount,
                        UnitPrice = property.UnitPrice,
                        CustomerId = customer.Id,
                        PropertyId = property.Id,
                        Units = investment.Units,
                        YearlyInterestAmount = (property.TargetYield * property.UnitPrice) / 100 * investment.Units,
                        Yield = property.TargetYield,
                        PaymentType = PaymentType.PROPERTY_PURCHASE.ToString(),
                        TransactionRef = Guid.NewGuid().ToString(),
                        Status = "PENDING"
                    };
                    if (!investmentRepository.AddInvestmentFromWallet(newInvestment)) return response;
                    var filePath = Path.Combine(env.ContentRootPath, @"EmailTemplates\investment.html");
                    var html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                    html = html.Replace("{FIRSTNAME}", string.Format("{0} {1}", customer.FirstName, customer.LastName))
                        .Replace("{I-NAME}", property.Name);
                    html = html.Replace("{I-UNITS}", investment.Units.ToString())
                        .Replace("{I-PRICE}", property.UnitPrice.ToString())
                        .Replace("{I-TOTAL}", newInvestment.Amount.ToString())
                        .Replace("{I-DATE}", newInvestment.PaymentDate.ToString())
                        .Replace("{webroot}", configuration["app_settings:WebEndpoint"]);
                    ;

                    EmailData emailData = new()
                    {
                        EmailBody = html,
                        EmailSubject = "Congratulations!!! You just made an investment",
                        EmailToId = customer.Email,
                        EmailToName = customer.FirstName
                    };
                    emailService.SendEmail(emailData);

                    response.Message = "You have successfully invested in the property";
                    response.Success = true;
                    return response;
                }
                case Channel.OFFLINE:
                {
                    OfflineInvestment off = new()
                    {
                        Amount = amount,
                        CustomerId = customer.Id,
                        PropertyId = property.Id,
                        Units = investment.Units,
                        PaymentRef = CommonLogic.GetUniqueRefNumber("INV"),
                        Status = "PENDING",
                        UnitPrice = property.UnitPrice,
                        CreatedDate = DateTime.Now
                    };
                    if (!investmentRepository.AddOfflineInvestment(off)) return response;
                    var filePath = Path.Combine(env.ContentRootPath, @"EmailTemplates\offline-payment.html");
                    var html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                    html = html.Replace("{FIRSTNAME}",
                            $"{customer.FirstName} {customer.LastName}")
                        .Replace("{webroot}", configuration["app_settings:WebEndpoint"])
                        .Replace("{BANKNAME}", configuration["app_settings:BankName"]).Replace("{ACCOUNTNAME}",
                            configuration["app_settings:AccountName"])
                        .Replace("{ACCOUNTNUMBER}", configuration["app_settings:AccountNumber"])
                        .Replace("{PAYREF}", off.PaymentRef);

                    EmailData emailRequest = new()
                    {
                        EmailBody = html,
                        EmailSubject = "Offline Investment Payment",
                        EmailToId = customer.Email,
                        EmailToName = customer.FirstName
                    };
                    emailService.SendEmail(emailRequest);

                    response.Message =
                        "You are one step into your investment, follow instructions in your email to complete your investment";
                    response.Success = true;
                    return response;
                }
                case Channel.CARD:
                case Channel.TRANSFER:
                {
                    Investment newInvestment = new()
                    {
                        Amount = amount,
                        CustomerId = customer.Id,
                        PropertyId = property.Id,
                        Units = investment.Units,
                        YearlyInterestAmount = (property.TargetYield * property.UnitPrice) / 100 * investment.Units,
                        Yield = property.TargetYield,
                        PaymentType = PaymentType.PROPERTY_PURCHASE.ToString(),
                        TransactionRef = Guid.NewGuid().ToString(),
                        Status = "PENDING"
                    };

                    if (investmentRepository.AddInvestment(newInvestment) == 0) return response;
                    Payment pay = new()
                    {
                        Amount = newInvestment.Amount,
                        Module = PaymentType.PROPERTY_PURCHASE,
                        InvestmentId = newInvestment.TransactionRef,
                        Callback = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/payment/verify-payment"
                    };
                    return paymentController.DoInitPayment(pay, customer.UniqueRef);
                }
                default:
                    return response;
            }
        }

        [HttpGet("{customerUniqueId}")]
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        public ApiResponse Investment(string customerUniqueId)
        {
            var investments = investmentRepository.GetInvestments(customerRepository.GetCustomer(customerUniqueId).Id)
                .Where(x => x.Status.Equals("COMPLETED")).ToList();
            List<InvestmentView> investmentViews = new();
            investments.ForEach(x =>
            {
                var xx = ClassConverter.ConvertInvestmentForView(x);
                var dd = uploadRepository.GetUploads(x.PropertyId);
                xx.Data = dd.Any(i => i.UploadType.Equals("COVER"))
                    ? dd.FirstOrDefault(y => y.UploadType.Equals("COVER"))!.Url
                    : "/images/properties/properties-4.jpg";
                investmentViews.Add(xx);
            });

            if (investments.Count > 0)
            {
                response.Message = "Successful";
                response.Success = true;
                response.Data = investmentViews;
                return response;
            }

            response.Message = "No record found";
            response.Data = investmentViews;
            return response;
        }

        [HttpGet("offline/{customerUniqueId}")]
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        public ApiResponse OfflineInvestment(string customerUniqueId)
        {
            var investments = investmentRepository
                .GetOfflineInvestments(customerRepository.GetCustomer(customerUniqueId).Id)
                .Where(x => x.Status.Equals(Status.PENDING.ToString())).ToList();

            if (investments.Any())
            {
                response.Message = "Successful";
                response.Success = true;
                response.Data = investments;
                return response;
            }

            response.Message = "No record found";
            response.Data = new List<OfflineInvestment>();
            return response;
        }

        [HttpGet("property/{propertyUniqueId}")]
        [Authorize(Roles = "ADMIN, SUPER")]
        public ApiResponse PropertyInvestment(string propertyUniqueId)
        {
            List<Investment> investments = investmentRepository
                .GetPropertyInvestments(propertyRepository.GetProperty(propertyUniqueId).Id)
                .Where(X => X.Status.Equals("COMPLETED")).ToList();
            List<InvestmentView> investments1 = new();
            investments.ForEach(x =>
            {
                var cc = customerRepository.GetCustomer(x.CustomerId);
                var xx = ClassConverter.ConvertInvestmentForView(x);
                xx.Customer = $"{cc.FirstName} {cc.LastName}";
                xx.Property = propertyRepository.GetProperty(x.PropertyId).Name;
                investments1.Add(xx);
            });

            if (investments.Count > 0)
            {
                response.Message = "Successful";
                response.Success = true;
                response.Data = investments1;
                return response;
            }

            response.Message = "No record found";
            return response;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, SUPER")]
        public ApiResponse Investment([FromQuery] QueryParams queryParams)
        {
            var investments1 = investmentRepository.GetInvestments(queryParams);
            var metadata = (
                investments1.TotalCount,
                investments1.PageSize,
                investments1.CurrentPage,
                investments1.TotalPages,
                investments1.HasNext,
                investments1.HasPrevious
            );

            List<InvestmentView> investments = new();
            investments1.ForEach(x =>
            {
                var cc = customerRepository.GetCustomer(x.CustomerId);
                var xx = ClassConverter.ConvertInvestmentForView(x);
                xx.Customer = $"{cc.FirstName} {cc.LastName}";
                xx.Property = propertyRepository.GetProperty(x.PropertyId).Name;
                investments.Add(xx);
            });

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            response.Success = true;
            response.Message = "Successful";
            response.Data = investments;
            return response;
        }

        [HttpPut("proof-of-payment/{investmentRef}")]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse UploadFile([FromBody] string uploadUrl, string investmentRef)
        {
            var investment = investmentRepository.GetOfflineInvestment(investmentRef);
            if (investment == null)
            {
                response.Message = "Investment not found, please login with your credentials and try again";
                return response;
            }

            investment.ProofUrl = uploadUrl;
            investment.Status = "PROCESSING";
            investment.PaymentDate = DateTime.Now;
            if (investmentRepository.UpdateOfflineInvestment(investment) != null)
            {
                response.Success = true;
                response.Message = "Proof of payment successfully uploaded";
                return response;
            }

            response.Message = "Error uploading proof of payment, please try again later";
            return response;
        }
    }
}