using System;
using System.Collections.Generic;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Models.Models;
using Newtonsoft.Json;
using Repositories.Repository;

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

        public InvestmentController(IPropertyRepository _propertyRepository, IStringLocalizer<InvestmentController> _localizer, IConfiguration _configuration,
        ICustomerRepository _customerRepository, IInvestmentRepository _investmentRepository, PaymentController _paymentController)
        {
            propertyRepository = _propertyRepository;
            localizer = _localizer;
            customerRepository = _customerRepository;
            investmentRepository = _investmentRepository;
            paymentController = _paymentController;
            configuration = _configuration;
        }
        
        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse Investments([FromBody] InvestmentNew investment)
        {
            var property = propertyRepository.GetProperty(investment.PropertyUniqueId);
            if(property == null){
                response.Message = $"Property with id {investment.PropertyUniqueId} not found";
                return response;
            }
            
            global::Models.Models.Customer customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);

            //if(investmentRepository.GetInvestments(customer.Id).Exists(x => x.PropertyId == property.Id))
            //{
            //    response.Message = $"You are already subscribed to the investment on this property {property.Name}";
            //    return response;
            //}

            if(investment.Units > property.MaxUnitPerCustomer)
            {
                response.Message = $"Maximum number of investment units of {property.MaxUnitPerCustomer} allowed per customer exceeded";
                return response;
            }

            Investment newInvestment = new()
            {
                Amount = property.UnitPrice * investment.Units,
                CustomerId = customer.Id,
                PropertyId = property.Id,
                Units = investment.Units,
                Yield = property.TargetYield,
                PaymentType = PaymentType.PROPERTY_PURCHASE.ToString(),
                TransactionRef = Guid.NewGuid().ToString()
            };
            if(newInvestment.Amount > decimal.Parse(configuration["app_settings:PayLimit"]))
            {
                //TODO write offline method code here
            }
            newInvestment.YearlyInterestAmount = newInvestment.Amount * newInvestment.Yield;
            long investmentId = investmentRepository.AddInvestment(newInvestment);
            if(investmentId != 0){
                Payment pay = new()
                {
                    Amount = newInvestment.Amount,
                    Module = PaymentType.PROPERTY_PURCHASE,
                    InvestmentId = investmentId,
                    Callback = string.Format("{0}/{1}", $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}", "api/payment/verify-payment")
                };
                return paymentController.DoInitPayment(pay, customer.UniqueRef);
            }
            return response;
        }

        //[HttpPut]
        //[Authorize(Roles="CUSTOMER")]
        //public ApiResponse Investments([FromBody] InvestmentUpdate investmentUpdate)
        //{
        //    var customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
        //    var investment = investmentRepository.GetInvestments(customer.Id).Where(x => x.Id == investmentUpdate.InvestmentId).FirstOrDefault();

        //    if (investment == null)
        //    {
        //        response.Message = "No such investment found for customer";
        //        return response;
        //    }

        //    investment.Units = investment.Units + investmentUpdate.Units;
        //    investment.Amount = investmentUpdate.Units * investment.Property.UnitPrice;

        //    Investment newInvestment = new()
        //    {
        //        Amount = property.UnitPrice * investment.Units,
        //        CustomerId = customer.Id,
        //        PropertyId = property.Id,
        //        Units = investment.Units,
        //        Yield = property.TargetYield,
        //        PaymentType = PaymentType.PROPERTY_PURCHASE.ToString(),
        //        TransactionRef = Guid.NewGuid().ToString()
        //    };
        //    newInvestment.YearlyInterestAmount = newInvestment.Amount * newInvestment.Yield;
        //    long investmentId = investmentRepository.AddInvestment(newInvestment);
        //    if (investmentId != 0)
        //    {
        //        Payment pay = new()
        //        {
        //            Amount = newInvestment.Amount,
        //            Module = PaymentType.PROPERTY_PURCHASE,
        //            InvestmentId = investmentId
        //        };
        //        return paymentController.doInitPayment(pay, customer.UniqueRef);
        //    }
        //    return response;
        //}

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

            if(investments.Count > 0){
                response.Message = "Successful";
                response.Success = true;
                response.Data = investments;
                return response;
            }
            response.Message = "No record found";
            return response;
        }

        [HttpGet]
        [Authorize]
        public ApiResponse Investment([FromQuery] QueryParams queryParams)
        {
            PagedList<Investment> investments = investmentRepository.GetInvestments(queryParams);
            (int TotalCount, int PageSize, int CurrentPage, int TotalPages, bool HasNext, bool HasPrevious) metadata = (
                investments.TotalCount,
                investments.PageSize,
                investments.CurrentPage,
                investments.TotalPages,
                investments.HasNext,
                investments.HasPrevious
            );
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            response.Success = true;
            response.Message = "Successfull";
            response.Data = investments;
            return response;
        }
    }
}