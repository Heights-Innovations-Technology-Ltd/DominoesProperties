using System;
using DominoesProperties.Models;

namespace DominoesProperties.Services
{
    public interface IEmailService
    {
        bool SendEmail(EmailData emailData);
        bool SendEmailWithAttachment(EmailDataWithAttachment emailData);
    }
}
