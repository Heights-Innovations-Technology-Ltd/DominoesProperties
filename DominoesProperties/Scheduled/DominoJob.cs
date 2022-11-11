using System;
using System.Threading.Tasks;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using MailKit.Net.Smtp;
using MimeKit;
using Models.Models;
using Repositories.Repository;

namespace DominoesProperties.Scheduled
{
    public class DominoJob : IDominoJob
    {
        private readonly EmailSettings _emailSettings;
        private readonly ICustomerRepository customerRepository;
        private readonly IEmailRetryRepository emailRetryRepository;
        private readonly IInvestmentRepository investmentRepository;
        private readonly ILoggerManager logger;
        private readonly IPropertyRepository propertyRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IWalletRepository walletRepository;

        public DominoJob(IInvestmentRepository _investmentRepository, IPropertyRepository _propertyRepository,
            ITransactionRepository _transactionRepository, IWalletRepository _walletRepository,
            IEmailRetryRepository _emailRetryRepository, ILoggerManager _logger,
            ICustomerRepository _customerRepository)
        {
            investmentRepository = _investmentRepository;
            propertyRepository = _propertyRepository;
            transactionRepository = _transactionRepository;
            walletRepository = _walletRepository;
            emailRetryRepository = _emailRetryRepository;
            logger = _logger;
            customerRepository = _customerRepository;
        }

        public void PerformPairInvestment()
        {
            investmentRepository.CompletedSharingGroup().ForEach(x =>
            {
                var tt = propertyRepository.GetProperty(x.PropertyId);
                if (tt.UnitAvailable > 0)
                {
                    using (var scope =
                           new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption
                               .RequiresNew))
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
                        catch (Exception e)
                        {
                            logger.LogError(e.StackTrace);
                            scope.Dispose();
                        }
                    }
                }
                else
                {
                    foreach (var y in x.Sharingentries)
                    {
                        using (var scope =
                               new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption
                                   .RequiresNew))
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
                            catch (Exception e)
                            {
                                logger.LogError(e.StackTrace);
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
                            using (var scope =
                                   new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption
                                       .RequiresNew))
                            {
                                try
                                {
                                    var amount = y.PercentageShare / 100 * tt.UnitPrice;

                                    Transaction transaction = new()
                                    {
                                        Amount = amount,
                                        Channel = Channel.TRANSFER.ToString(),
                                        CustomerId = y.CustomerId,
                                        Module = PaymentType.REVERSAL.ToString(),
                                        Status = "success",
                                        TransactionRef = $"RV||{y.PaymentReference}",
                                        TransactionType = TransactionType.DR.ToString()
                                    };

                                    transactionRepository.NewTransaction(transaction);

                                    y.Customer.Wallet.Balance += amount;
                                    walletRepository.UpdateCustomerWallet(y.Customer.Wallet);

                                    y.IsReversed = true;
                                    y.IsClosed = true;
                                    x.IsClosed = true;
                                    investmentRepository.UpdateSharingGroup(x);
                                    scope.Complete();
                                }
                                catch (Exception e)
                                {
                                    logger.LogError(e.StackTrace);
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

        public void ResendEmail()
        {
            SmtpClient emailClient = new();
            try
            {
                emailClient.Connect(_emailSettings.Host, _emailSettings.Port, _emailSettings.UseSSL);
                emailClient.Authenticate(_emailSettings.EmailId, _emailSettings.Password);

                var retries = emailRetryRepository.GetRetries();
                retries.ForEach(x =>
                {
                    if (x.DateCreated.Day == DateTime.Now.Day && x.DateCreated.AddMinutes(5) <= DateTime.Now &&
                        x.RetryCount < 3)
                    {
                        MimeMessage emailMessage = new();

                        MailboxAddress emailFrom = new(_emailSettings.Name, _emailSettings.EmailId);
                        emailMessage.From.Add(emailFrom);

                        MailboxAddress emailTo = new(x.RecipientName, x.Recipient);
                        emailMessage.To.Add(emailTo);

                        emailMessage.Subject = x.Subject;

                        BodyBuilder emailBodyBuilder = new()
                        {
                            HtmlBody = x.Body
                        };
                        emailMessage.Body = emailBodyBuilder.ToMessageBody();
                        emailClient.Send(emailMessage);

                        x.DateCreated = DateTime.Now;
                        x.StatusCode = "200";
                        x.RetryCount += 1;

                        emailRetryRepository.UpdateRetry(x);
                    }
                });

                emailClient.Disconnect(true);
                emailClient.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogDebug("Connection not obtained for SMTP server");
                logger.LogDebug(ex.StackTrace);
                emailClient.Disconnect(true);
                emailClient.Dispose();
            }
        }

        public void CheckSubscription()
        {
            var xx = customerRepository.GetCustomers().FindAll(x =>
                x.NextSubscriptionDate.HasValue && x.NextSubscriptionDate.Value.CompareTo(DateTime.Now) < 0 &&
                x.IsSubscribed.Value);
            xx.ForEach(x => { x.IsSubscribed = false; });

            customerRepository.UpdateCustomers(xx);
        }

        public void ClearPendingInvestments()
        {
            investmentRepository.DeletePendingInvestments();
        }
    }
}