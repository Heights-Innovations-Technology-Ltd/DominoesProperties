using System;
using System.Collections.Generic;
using System.Linq;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class InvestmentService : BaseRepository, IInvestmentRepository
    {
        public InvestmentService(dominoespropertiesContext context):base(context)
        {
        }

        public long AddInvestment(Investment investment)
        {
            _context.Investments.Add(investment);
            _context.SaveChanges();
            return investment.Id;
        }

        public Investment GetInvestment(long Id)
        {
            return _context.Investments.Find(Id);
        }

        public List<Investment> GetInvestments(long customerId)
        {
            return _context.Investments.Where(x => x.CustomerId.Equals(customerId)).ToList();
        }

        public void UpdateInvestment(Investment investment)
        {
            _context.Investments.Update(investment);
            _context.SaveChanges();
        }
    }
}
