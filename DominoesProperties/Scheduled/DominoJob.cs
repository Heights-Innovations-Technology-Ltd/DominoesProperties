using System;
using System.Threading.Tasks;
using DominoesProperties.Enums;
using Models.Models;
using Repositories.Repository;

namespace DominoesProperties.Scheduled
{
    public class DominoJob : IDominoJob
    {
        private readonly IInvestmentRepository investmentRepository;
        private readonly IPropertyRepository propertyRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IWalletRepository walletRepository;

        public DominoJob(IInvestmentRepository _investmentRepository, IPropertyRepository _propertyRepository, ITransactionRepository _transactionRepository, IWalletRepository _walletRepository)
        {
            investmentRepository = _investmentRepository;
            propertyRepository = _propertyRepository;
            transactionRepository = _transactionRepository;
            walletRepository = _walletRepository;
        }

        public void PerformPairInvestment()
        {
            investmentRepository.CompletedSharingGroup().ForEach(x =>
            {
                var tt = propertyRepository.GetProperty(x.PropertyId);
                if (tt.UnitAvailable > 0)
                {
                    using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew))
                    {
                        try
                        {
                            tt.UnitAvailable--;
                            x.IsClosed = true;
                            foreach (var t in x.Sharingentries)
                            {
                                t.IsClosed = true;
                            }
                            propertyRepository.UpdateProperty(tt);
                            investmentRepository.UpdateSharingGroup(x);
                            scope.Complete();
                        }
                        catch (Exception)
                        {
                            scope.Dispose();
                        }
                    }
                }
                else
                {
                    foreach (var y in x.Sharingentries)
                    {
                        using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew))
                        {
                            try
                            {
                                var amount = y.PercentageShare / 100 * tt.UnitPrice;

                                Transaction transaction = new();
                                transaction.Amount = amount;
                                transaction.Channel = Channel.TRANSFER.ToString();
                                transaction.CustomerId = y.CustomerId;
                                transaction.Module = PaymentType.REVERSAL.ToString();
                                transaction.Status = "success";
                                transaction.TransactionRef = $"RV||{y.PaymentReference}";
                                transaction.TransactionType = TransactionType.DR.ToString();

                                transactionRepository.NewTransaction(transaction);

                                y.Customer.Wallet.Balance += amount;
                                walletRepository.UpdateCustomerWallet(y.Customer.Wallet);

                                y.IsReversed = true;
                                y.IsClosed = true;
                                x.IsClosed = true;
                                investmentRepository.UpdateSharingGroup(x);
                                scope.Complete();
                            }
                            catch (Exception)
                            {
                                scope.Dispose();
                            }
                        }

                    }
                }
            });
        }

        public Task<bool> CheckUnclosedPairing()
        {
            Task<bool> finished = Task<bool>.Factory.StartNew(() =>
            {
                investmentRepository.UncompletedSharingGroup().ForEach(x =>
                {
                    var tt = propertyRepository.GetProperty(x.PropertyId);
                    if (tt.UnitAvailable < 1)
                    {
                        foreach (var y in x.Sharingentries)
                        {
                            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew))
                            {
                                try
                                {
                                    var amount = y.PercentageShare / 100 * tt.UnitPrice;

                                    Transaction transaction = new();
                                    transaction.Amount = amount;
                                    transaction.Channel = Channel.TRANSFER.ToString();
                                    transaction.CustomerId = y.CustomerId;
                                    transaction.Module = PaymentType.REVERSAL.ToString();
                                    transaction.Status = "success";
                                    transaction.TransactionRef = $"RV||{y.PaymentReference}";
                                    transaction.TransactionType = TransactionType.DR.ToString();

                                    transactionRepository.NewTransaction(transaction);

                                    y.Customer.Wallet.Balance += amount;
                                    walletRepository.UpdateCustomerWallet(y.Customer.Wallet);

                                    y.IsReversed = true;
                                    y.IsClosed = true;
                                    x.IsClosed = true;
                                    investmentRepository.UpdateSharingGroup(x);
                                    scope.Complete();
                                }
                                catch (Exception)
                                {
                                    scope.Dispose();
                                }
                            }
                        }
                    }
                });
                return true;
            });
            return finished;
        }
    }
}