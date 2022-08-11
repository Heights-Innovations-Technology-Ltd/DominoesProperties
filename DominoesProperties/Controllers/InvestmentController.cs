using System;
using System.Collections.Generic;
using System.IO;
using DominoesProperties.Enums;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using DominoesProperties.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Models.Context;
using Models.Models;
using Newtonsoft.Json;
using Repositories.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Transaction = Microsoft.EntityFrameworkCore.DbLoggerCategory.Database.Transaction;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : Controller
    {
        private readonly ApiResponse response = new ApiResponse(false, "Error performing request, contact admin");
        private readonly IPropertyRepository propertyRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IInvestmentRepository investmentRepository;
        private readonly IStringLocalizer<InvestmentController> localizer;
        private readonly PaymentController paymentController;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;
        private readonly IEmailService emailService;

        public InvestmentController(IPropertyRepository _propertyRepository, IStringLocalizer<InvestmentController> _localizer, IConfiguration _configuration,
        ICustomerRepository _customerRepository, IInvestmentRepository _investmentRepository, PaymentController _paymentController, IWebHostEnvironment _env,
        IEmailService _emailService)
        {
            propertyRepository = _propertyRepository;
            localizer = _localizer;
            customerRepository = _customerRepository;
            investmentRepository = _investmentRepository;
            paymentController = _paymentController;
            configuration = _configuration;
            env = _env;
            emailService = _emailService;
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

            global::Models.Models.Customer customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);

            if (investment.Units > property.UnitAvailable)
            {
                response.Message = $"Not enough investment units available, only {property.UnitAvailable} units available for purchase";
                return response;
            }

            if (investment.Units > property.MaxUnitPerCustomer)
            {
                response.Message = $"Maximum number of investment units of {property.MaxUnitPerCustomer} allowed per customer exceeded";
                return response;
            }

            var amount = property.UnitPrice * investment.Units;

            if (investment.Channel.Equals(Channel.WALLET))
            {
                if (customer.Wallet.Balance < amount)
                {
                    response.Message = "Low wallet balance, please fund your wallet or try a different payment method to complete your investment.";
                    return response;
                }

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
                if (investmentRepository.AddInvestmentFromWallet(newInvestment))
                {
                    string filePath = Path.Combine(env.ContentRootPath, @"EmailTemplates\investment.html");
                    string html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                    html = html.Replace("{FIRSTNAME}", string.Format("{0} {1}", customer.FirstName, customer.LastName)).Replace("{I-NAME}", property.Name);
                    html = html.Replace("{I-UNITS}", investment.Units.ToString()).Replace("{I-PRICE}", property.UnitPrice.ToString()).Replace("{I-TOTAL}", newInvestment.Amount.ToString()).Replace("{I-DATE}", newInvestment.PaymentDate.ToString()).Replace("{webroot}", configuration["app_settings:WebEndpoint"]); ;

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
            }
            else
            {
                if (amount > decimal.Parse(configuration["app_settings:PayLimit"]))
                {
                    //TODO write offline method code here
                }
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

                if (investmentRepository.AddInvestment(newInvestment) != 0)
                {
                    Payment pay = new()
                    {
                        Amount = newInvestment.Amount,
                        Module = PaymentType.PROPERTY_PURCHASE,
                        InvestmentId = newInvestment.TransactionRef,
                        Callback = string.Format("{0}/{1}", $"{Request.Scheme}://{Request.Host}{Request.PathBase}", "api/payment/verify-payment")
                    };
                    return paymentController.DoInitPayment(pay, customer.UniqueRef);
                }
            }
            return response;
        }

        [HttpGet("{customerUniqueId}")]
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        public ApiResponse Investment(string customerUniqueId)
        {
            List<Investment> investments = investmentRepository.GetInvestments(customerRepository.GetCustomer(customerUniqueId).Id);
            investments.ForEach(x =>
            {
                x.Customer = null;
                x.Property = null;
            });

            if (investments.Count > 0)
            {
                response.Message = "Successful";
                response.Success = true;
                response.Data = investments;
                return response;
            }
            response.Message = "No record found";
            return response;
        }

        [HttpGet("property/{propertyUniqueId}")]
        [Authorize(Roles = "ADMIN, SUPER")]
        public ApiResponse PropertyInvestment(string propertyUniqueId)
        {
            List<Investment> investments = investmentRepository.GetPropertyInvestments(propertyRepository.GetProperty(propertyUniqueId).Id);
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
            PagedList<Investment> investments1 = investmentRepository.GetInvestments(queryParams);
            (int TotalCount, int PageSize, int CurrentPage, int TotalPages, bool HasNext, bool HasPrevious) metadata = (
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
            response.Message = "Successfull";
            response.Data = investments;
            return response;
        }
    }
}
