using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class InvestmentService : BaseRepository, IInvestmentRepository
    {
        private readonly ILoggerManager loggerManager;

        public InvestmentService(dominoespropertiesContext context, ILoggerManager _loggerManager) : base(context)
        {
            loggerManager = _loggerManager;
        }

        public long AddInvestment(Investment investment)
        {
            try
            {
                var inv = _context.Investments.Add(investment);
                _context.SaveChanges();
                var id = inv.Entity.Id;
                loggerManager.LogInfo($"Investment successfully saved with id {id}");
                return id;
            }
            catch (Exception ex)
            {
                loggerManager.LogInfo($"Investment failed with id {ex.StackTrace}");
                loggerManager.LogError(ex.StackTrace);
                return 0;
            }
        }

        public Investment GetInvestment(long Id)
        {
            return _context.Investments.Find(Id);
        }

        public List<Investment> GetInvestments(long customerId)
        {
            return _context.Investments
                .Include(x => x.Property)
                .Where(x => x.CustomerId.Equals(customerId) && x.Status.Equals("COMPLETED")).ToList();
        }

        public IEnumerable<Investment> GetPropertyInvestments(long propertyId)
        {
            return _context.Investments.Where(x => x.PropertyId.Equals(propertyId) && x.Status.Equals("COMPLETED"))
                .ToList();
        }

        public Investment GetNewInvestments(string transactionRef)
        {
            return _context.Investments
                .Include(x => x.Property)
                .FirstOrDefault(x => x.TransactionRef.Equals(transactionRef));
        }

        public PagedList<Investment> GetInvestments(QueryParams pageParams)
        {
            return PagedList<Investment>.ToPagedList(
                _context.Investments.Where(x => x.Status.Equals("COMPLETED")).OrderBy(on => on.Id),
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
            return _context.Investments.Where(x => x.PropertyId == propertyId && x.Status.Equals("COMPLETED"))
                .Distinct().SelectMany(x => x.Customer.Email).ToList();
        }

        public bool AddInvestmentFromWallet(Investment investment)
        {
            using var tt = _context.Database.BeginTransaction();
            try
            {
                var wallet = _context.Wallets.FirstOrDefault(x => x.CustomerId == investment.CustomerId);
                wallet.Balance -= investment.Amount;
                wallet.LastTransactionDate = DateTime.Now;
                wallet.LastTransactionAmount = -investment.Amount;
                _context.Wallets.Update(wallet);

                Transaction transaction = new()
                {
                    Amount = investment.Amount,
                    Channel = "WALLET",
                    CustomerId = investment.CustomerId,
                    Module = "PROPERTY_PURCHASE",
                    Status = "success",
                    TransactionRef = investment.TransactionRef,
                    TransactionType = "CR"
                };
                _context.Transactions.Add(transaction);

                investment.Status = "COMPLETED";
                var property = _context.Properties.FirstOrDefault(x => x.Id == investment.PropertyId);
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

        public List<Sharinggroup> GetSharinggroups(long propertyId)
        {
            return _context.Sharinggroups
                .Include(x => x.Sharingentries)
                .Where(x => !x.IsClosed.Value)
                .ToList();
        }

        public Sharinggroup GetSharinggroups(string groupRef)
        {
            return _context.Sharinggroups
                .FirstOrDefault(x => x.UniqueId == groupRef);
        }

        public List<Sharinggroup> CompletedSharingGroup()
        {
            return _context.Sharinggroups
                .Where(x => x.PercentageSubscribed == 100 && x.IsClosed.Value && !x.IsInvested)
                .Include(x => x.Sharingentries)
                .ToList();
        }

        public List<Sharinggroup> UncompletedSharingGroup()
        {
            return _context.Sharinggroups
                .Where(x => !x.IsClosed.Value)
                .ToList();
        }

        public bool AddSharingentry(Sharingentry sharingentry)
        {
            try
            {
                _context.Sharingentries.Add(sharingentry);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddSharingGroup(Sharinggroup sharinggroup)
        {
            try
            {
                _context.Sharinggroups.Add(sharinggroup);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void UpdateSharingGroup(Sharinggroup sharinggroup)
        {
            _context.Sharinggroups.Update(sharinggroup);
            _context.SaveChanges();
        }

        public async Task<bool> CloseSharingGroupAsync(long groupId)
        {
            try
            {
                var group = _context.Sharinggroups.Find(groupId);
                group.IsClosed = true;
                await group.Sharingentries.AsQueryable().ForEachAsync(x => x.IsClosed = true);
                _context.Sharinggroups.Update(group);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                loggerManager.LogError(ex.ToString());
                return false;
            }
        }

        public void DeleteGroup(Sharinggroup groupRef)
        {
            try
            {
                _context.Sharinggroups.Remove(groupRef);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                loggerManager.LogError(ex.ToString());
            }
        }

        public IEnumerable<Sharingentry> GetSharingEntries(long customerId)
        {
            return _context.Sharingentries
                .Where(x => x.CustomerId == customerId).ToList();
        }

        public void DeletePendingInvestments()
        {
            var inv = _context.Investments.Where(x => x.Status.Equals("PENDING"));
            if (inv.Any())
                _context.Investments.RemoveRange(inv);
            _context.SaveChanges();
        }

        public IEnumerable<OfflineInvestment> GetOfflineInvestments(long customerId)
        {
            return _context.OfflineInvestments.Where(x => x.CustomerId == customerId);
        }

        public async Task<object> GetOfflineInvestments()
        {
            var offlineInvestments = await _context.OfflineInvestments
                .ToListAsync();

            return offlineInvestments.GroupBy(t => t.Status)
                .Select(b => new { group = b.Key, items = b });
        }

        public OfflineInvestment GetOfflineInvestment(long id)
        {
            return _context.OfflineInvestments.Find(id);
        }

        public OfflineInvestment GetOfflineInvestment(string paymentRef)
        {
            return _context.OfflineInvestments.FirstOrDefault(x => x.PaymentRef.Equals(paymentRef));
        }

        public bool AddOfflineInvestment(OfflineInvestment investment)
        {
            try
            {
                _context.OfflineInvestments.Add(investment);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public OfflineInvestment UpdateOfflineInvestment(OfflineInvestment investment)
        {
            _context.OfflineInvestments.Update(investment);
            _context.SaveChanges();
            return investment;
        }
    }
}