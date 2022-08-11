using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models.Context;
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
            return _context.Investments
                .Include(x => x.Property)
                .Where(x => x.CustomerId.Equals(customerId)).ToList();
        }

        public List<Investment> GetPropertyInvestments(long propertyId)
        {
            return _context.Investments.Where(x => x.PropertyId.Equals(propertyId)).ToList();
        }

        public Investment GetNewInvestments(string transactionRef)
        {
            return _context.Investments
                .Include(x => x.Property)
                .Where(x => x.TransactionRef.Equals(transactionRef)).FirstOrDefault();
        }

        public PagedList<Investment> GetInvestments(QueryParams pageParams)
        {
            return PagedList<Investment>.ToPagedList(_context.Investments.OrderBy(on => on.Id),
                pageParams.PageNumber,
                pageParams.PageSize);
        }

        public void UpdateInvestment(Investment investment)
        {
            _context.Investments.Update(investment);
            _context.SaveChanges();
        }

        public List<char> GetUsersOnInvestment(long propertyId)
        {
            return _context.Investments.Where(x => x.PropertyId == propertyId).Distinct().SelectMany(x => x.Customer.Email).ToList();
        }

        public bool AddInvestmentFromWallet(Investment investment)
        {
            using var tt = _context.Database.BeginTransaction();
            try
            {
                var wallet = _context.Wallets.Where(x => x.CustomerId == investment.CustomerId).FirstOrDefault();
                wallet.Balance -= investment.Amount;
                wallet.LastTransactionDate = DateTime.Now;
                wallet.LastTransactionAmount = -investment.Amount;
                _context.Wallets.Update(wallet);

                Transaction transaction = new();
                transaction.Amount = investment.Amount;
                transaction.Channel = "wallet";
                transaction.CustomerId = investment.CustomerId;
                transaction.Module = "PROPERTY_PURCHASE";
                transaction.Status = "success";
                transaction.TransactionRef = investment.TransactionRef;
                transaction.TransactionType = "CR";
                _context.Transactions.Add(transaction);

                investment.Status = "COMPLETED";
                Property property = _context.Properties.Where(x => x.Id == investment.PropertyId).FirstOrDefault();
                property.UnitAvailable -= investment.Units;
                property.UnitSold += investment.Units;

                if (property.UnitAvailable == 0)
                {
                    property.Status = "CLOSED_FOR_INVESTMENT";
                }
                _context.Properties.Update(property);
                _context.Investments.Add(investment);

                tt.Commit();
                return true;
            }
            catch (Exception)
            {
                tt.Rollback();
                return false;
            }
        }
    }
}
