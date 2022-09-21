using System;
using System.IO;
using System.Net.Sockets;
using DominoesProperties.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using Models.Models;
using Repositories.Repository;

namespace DominoesProperties.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILoggerManager logger;
        private readonly IEmailRetryRepository emailRetrials;
        public EmailService(IOptions<EmailSettings> options, ILoggerManager _logger, IEmailRetryRepository _emailRetrials)
        {
            _emailSettings = options.Value;
            logger = _logger;
            emailRetrials = _emailRetrials;
        }

        public bool SendEmail(EmailData emailData)
        {
            try
            {
                MimeMessage emailMessage = new();

                MailboxAddress emailFrom = new(_emailSettings.Name, _emailSettings.EmailId);
                emailMessage.From.Add(emailFrom);

                MailboxAddress emailTo = new(emailData.EmailToName, emailData.EmailToId);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = emailData.EmailSubject;

                BodyBuilder emailBodyBuilder = new()
                {
                    HtmlBody = emailData.EmailBody
                };
                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                SmtpClient emailClient = new();
                emailClient.Connect(_emailSettings.Host, _emailSettings.Port, _emailSettings.UseSSL);
                emailClient.Authenticate(_emailSettings.EmailId, _emailSettings.Password);
                emailClient.Send(emailMessage);
                emailClient.Disconnect(true);
                emailClient.Dispose();

                return true;
            }
            catch (SocketException ex)
            {
                //Log Exception Details
                logger.LogError(ex.StackTrace);
                EmailRetry retry = new()
                {
                    Subject = emailData.EmailSubject,
                    Body = emailData.EmailBody,
                    DateCreated = DateTime.Now,
                    Recipient = emailData.EmailToId,
                    RecipientName = emailData.EmailToName,
                    StatusCode = "500",
                    Category = "CUSTOMER"
                };
                emailRetrials.AddRetry(retry);
                return false;
            }catch(Exception ex)
            {
                logger.LogError(ex.StackTrace);
                return false;
            }
        }

        public bool SendEmailWithAttachment(EmailDataWithAttachment emailData)
        {
            try
            {
                MimeMessage emailMessage = new();

                MailboxAddress emailFrom = new(_emailSettings.Name, _emailSettings.EmailId);
                emailMessage.From.Add(emailFrom);

                MailboxAddress emailTo = new(emailData.EmailToName, emailData.EmailToId);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = emailData.EmailSubject;

                BodyBuilder emailBodyBuilder = new BodyBuilder();

                if (emailData.EmailAttachments != null)
                {
                    byte[] attachmentFileByteArray;
                    foreach (IFormFile attachmentFile in emailData.EmailAttachments)
                    {
                        if (attachmentFile.Length > 0)
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                attachmentFile.CopyTo(memoryStream);
                                attachmentFileByteArray = memoryStream.ToArray();
                            }
                            emailBodyBuilder.Attachments.Add(attachmentFile.FileName, attachmentFileByteArray, ContentType.Parse(attachmentFile.ContentType));
                        }
                    }
                }

                emailBodyBuilder.TextBody = emailData.EmailBody;
                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                SmtpClient emailClient = new SmtpClient();
                emailClient.Connect(_emailSettings.Host, _emailSettings.Port, _emailSettings.UseSSL);
                emailClient.Authenticate(_emailSettings.EmailId, _emailSettings.Password);
                emailClient.Send(emailMessage);
                emailClient.Disconnect(true);
                emailClient.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                //Log Exception Details
                logger.LogError(ex.StackTrace);
                return false;
            }
        }
    }
}
