﻿using System;
using System.Threading.Tasks;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
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
            ICustomerRepository _customerRepository, IOptions<EmailSettings> options)
        {
            investmentRepository = _investmentRepository;
            propertyRepository = _propertyRepository;
            transactionRepository = _transactionRepository;
            walletRepository = _walletRepository;
            emailRetryRepository = _emailRetryRepository;
            logger = _logger;
            customerRepository = _customerRepository;
            _emailSettings = options.Value;
        }

        public void PerformPairInvestment()
        {
            var xxx = investmentRepository.CompletedSharingGroup();
            xxx.ForEach(x =>
            {
                logger.LogInfo("Pairing completion started++++++++++");
                var tt = propertyRepository.GetProperty(x.PropertyId);
                if (tt.UnitAvailable > 0)
                {
                    logger.LogInfo($"Pairing started with avaialable units on property {tt.Name} ++++++++++");
                    using (var scope =
                           new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption
                               .RequiresNew))
                    {
                        try
                        {
                            tt.UnitAvailable--;
                            x.IsInvested = true;
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

                    logger.LogInfo($"Pairing completed with on property {tt.Name} ++++++++++");
                }
                else
                {
                    logger.LogInfo($"Pairing reversal started property {tt.Name}, units exhausted ++++++++++");
                    foreach (var y in x.Sharingentries)
                    {
                        using (var scope =
                               new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption
                                   .RequiresNew))
                        {
                            try
                            {
                                var amount = decimal.Divide(y.PercentageShare, 100) * tt.UnitPrice;

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

                    logger.LogInfo($"Pairing reversal completed on property {tt.Name} ++++++++++");
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
                    if (x.DateCreated.Day != DateTime.Now.Day || DateTime.Now > x.DateCreated.AddMinutes(5) ||
                        x.RetryCount >= 3) return;
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
                x.IsSubscribed != null && x.NextSubscriptionDate.HasValue &&
                x.NextSubscriptionDate.Value.CompareTo(DateTime.Now) < 0 &&
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