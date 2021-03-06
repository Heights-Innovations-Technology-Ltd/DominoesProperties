using System;
using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IInvestmentRepository
    {
        long AddInvestment(Investment investment);
        void UpdateInvestment(Investment investment);
        List<Investment> GetInvestments(long customerId);
        PagedList<Investment> GetInvestments(QueryParams queryParams);
        Investment GetInvestment(long investmentId);
    }
}
