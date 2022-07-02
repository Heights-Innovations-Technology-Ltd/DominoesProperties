using System;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public InvestmentController(IPropertyRepository _propertyRepository, IStringLocalizer<InvestmentController> _localizer, 
        ICustomerRepository _customerRepository, IInvestmentRepository _investmentRepository, PaymentController _paymentController)
        {
            propertyRepository = _propertyRepository;
            localizer = _localizer;
            customerRepository = _customerRepository;
            investmentRepository = _investmentRepository;
            paymentController = _paymentController;
        }
        
        [HttpPost]
        [Authorize]
        public ApiResponse Investments([FromBody] InvestmentNew investment)
        {
            var property = propertyRepository.GetProperty(investment.PropertyUniqueId);
            if(property == null){
                response.Message = $"Property with id {investment.PropertyUniqueId} not found";
                return response;
            }
            global::Models.Models.Customer customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
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
            newInvestment.YearlyInterestAmount = newInvestment.Amount * newInvestment.Yield;
            long investmentId = investmentRepository.AddInvestment(newInvestment);
            if(investmentId != 0){
                Payment pay = new()
                {
                    Amount = newInvestment.Amount,
                    Module = Enums.PaymentType.PROPERTY_PURCHASE,
                    InvestmentId = investmentId
                };
                return paymentController.doInitPayment(pay, customer.UniqueRef);
            }
            return response;
        }

        [HttpGet("{customerId}")]
        [Authorize(Roles = "Admin")]
        public ApiResponse Investment(string customerId){

            System.Collections.Generic.List<Investment> investments = investmentRepository.GetInvestments(customerRepository.GetCustomer(customerId).Id);
            if(investments.Count > 1){
                response.Message = "Successful";
                response.Data = investments;
                return response;
            }
            response.Message = "No record found";
            return response;
        }

        [HttpGet]
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