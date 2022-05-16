using System.Linq;
using DominoesProperties.Models;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Models.Models;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : Controller
    {
        private readonly ApiResponse response = new ApiResponse(false, "Error performing request, contact admin");
        private readonly ILoggerManager logger;
        private readonly IPropertyRepository propertyRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IInvestmentRepository investmentRepository;
        private readonly IStringLocalizer<InvestmentController> localizer;
        private readonly PaymentController paymentController;

        public InvestmentController(ILoggerManager _logger, IPropertyRepository _propertyRepository, IStringLocalizer<InvestmentController> _localizer, 
        ICustomerRepository _customerRepository, IInvestmentRepository _investmentRepository, PaymentController _paymentController){
            logger = _logger;
            propertyRepository = _propertyRepository;
            localizer = _localizer;
            customerRepository = _customerRepository;
            investmentRepository = _investmentRepository;
            paymentController = _paymentController;
        }
        
        [HttpPost]
        [Authorize]
        public ApiResponse Investment([FromBody] InvestmentNew investment)
        {
            var property = propertyRepository.GetProperty(investment.PropertyUniqueId);
            if(property == null){
                response.Message = localizer["Property.Not.Found"];
                return response;
            }
            var customer = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
            Investment newInvestment = new Investment();
            newInvestment.Amount = property.UnitPrice * investment.Units;
            newInvestment.CustomerId = customer.Id;
            newInvestment.PropertyId = property.Id;
            newInvestment.Units = investment.Units;
            newInvestment.Yield = property.TargetYield;
            newInvestment.YearlyInterestAmount = newInvestment.Amount * newInvestment.Yield;
            var investmentId = investmentRepository.AddInvestment(newInvestment);
            if(investmentId != 0){
                Payment pay = new Payment();
                pay.Amount =  newInvestment.Amount;
                pay.Module = Enums.PaymentType.PROPERTY_PURCHASE;
                pay.InvestmentId = investmentId;
                paymentController.InitiateTransaction(pay);
            }
            return response;
        }
    }
}