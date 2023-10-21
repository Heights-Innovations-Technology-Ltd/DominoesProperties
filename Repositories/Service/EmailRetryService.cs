using System;
using System.Collections.Generic;
using System.Linq;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class EmailRetryService : BaseRepository, IEmailRetryRepository, IDisposable
    {
        private readonly ILoggerManager logger;

        public EmailRetryService(dominoespropertiesContext context, ILoggerManager _logger) : base(context)
        {
            logger = _logger;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void AddRetry(EmailRetry email)
        {
            try
            {
                _context.Emailretries.Add(email);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public List<EmailRetry> GetRetries()
        {
            try
            {
                return _context.Emailretries
                    .Where(x => !x.StatusCode.Equals("200") && x.DateCreated.Date == DateTime.Today.Date &&
                                x.RetryCount < 3).OrderBy(x => x.DateCreated).ToList();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new List<EmailRetry>();
            }
        }

        public void UpdateRetry(EmailRetry email)
        {
            try
            {
                _context.Emailretries.Update(email);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }
    }
}