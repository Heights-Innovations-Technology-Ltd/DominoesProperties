using System;
using System.IO;
using DominoesProperties.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using Repositories.Repository;

namespace DominoesProperties.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILoggerManager logger;
        public EmailService(IOptions<EmailSettings> options, ILoggerManager _logger)
        {
            _emailSettings = options.Value;
            logger = _logger;
        }

        public bool SendEmail(EmailData emailData)
        {
            try
            {
                MimeMessage emailMessage = new MimeMessage();

                MailboxAddress emailFrom = new MailboxAddress(_emailSettings.Name, _emailSettings.EmailId);
                emailMessage.From.Add(emailFrom);

                MailboxAddress emailTo = new MailboxAddress(emailData.EmailToName, emailData.EmailToId);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = emailData.EmailSubject;

                BodyBuilder emailBodyBuilder = new()
                {
                    HtmlBody = emailData.EmailBody
                };
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
