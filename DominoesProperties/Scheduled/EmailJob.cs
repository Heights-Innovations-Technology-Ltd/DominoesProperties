using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DominoesProperties.Scheduled
{
    [DisallowConcurrentExecution]
    public class EmailJob : IJob
    {
        private readonly IDominoJob _dominoJob;
        private readonly ILogger _logger;

        public EmailJob(IDominoJob dominoJob, ILogger logger)
        {
            _dominoJob = dominoJob;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                _ = Start();
                _dominoJob.ResendEmail();
                return Task.CompletedTask;
            }
            catch (JobExecutionException e)
            {
                _logger.LogError(e, "Error executing job with exception {EMessage}", e.Message);
                return Task.CompletedTask;
            }
        }

        private static Task Start()
        {
            return Task.CompletedTask;
        }
    }
}