using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IEmailRetryRepository
    {
        void AddRetry(EmailRetry email);
        List<EmailRetry> GetRetries();
        void UpdateRetry(EmailRetry email);
    }
}
