using System;
using Hangfire;
using Models.Models;
using Repositories.Repository;

namespace DominoesProperties.Scheduled
{
    public class DominoJobs
    {
        private readonly IRecurringJobManager backgroundJob;
        private readonly IInvestmentRepository investmentRepository;
        public DominoJobs(IRecurringJobManager _backgroundJob, IInvestmentRepository _investmentRepository)
        {
            backgroundJob = _backgroundJob;
            investmentRepository = _investmentRepository;
        }

        protected void PerformPairInvestment()
        {
            investmentRepository.CompletedSharingGroup().ForEach(x => {
                Investment newInvestment = new()
                {
                    Amount = x.Property.UnitPrice,
                    CustomerId = customer.Id,
                    PropertyId = property.Id,
                    Units = investment.Units,
                    YearlyInterestAmount = (property.TargetYield * property.UnitPrice) / 100 * investment.Units,
                    Yield = property.TargetYield,
                    PaymentType = PaymentType.PROPERTY_PURCHASE.ToString(),
                    TransactionRef = Guid.NewGuid().ToString(),
                    Status = "PENDING"
                };
            });
        }
    }
}

