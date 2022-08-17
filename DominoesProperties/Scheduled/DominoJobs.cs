using System;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Repositories.Repository;

namespace DominoesProperties.Scheduled
{
    public class DominoJobs : Controller
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
                
            });
        }
    }
}

