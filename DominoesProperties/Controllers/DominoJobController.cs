using DominoesProperties.Scheduled;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace DominoesProperties.Controllers
{
    [ApiController]
    public class DominoJobs : Controller
    {
        private readonly IRecurringJobManager backgroundJob;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IDominoJob dominoJob;

        public DominoJobs(IRecurringJobManager _backgroundJob, IBackgroundJobClient _backgroundJobClient,
            IDominoJob _dominoJob)
        {
            backgroundJob = _backgroundJob;
            backgroundJobClient = _backgroundJobClient;
            dominoJob = _dominoJob;
        }

        [HttpGet("complete-pairing")]
        public void CompletePairing()
        {
            backgroundJob.AddOrUpdate("complete-pairing", () => dominoJob.PerformPairInvestment(), Cron.Hourly(30));
        }

        [HttpGet("incomplete-pairing")]
        public void UncompletedPairing()
        {
            backgroundJobClient.ContinueJobWith("complete-pairing", () => dominoJob.CheckUnclosedPairing(),
                JobContinuationOptions.OnlyOnSucceededState);
        }

        [HttpGet("retry-mail")]
        public void RetryEmailSender()
        {
            backgroundJob.AddOrUpdate("retry-email", () => dominoJob.ResendEmail(), "*/10 * * * *");
        }

        [HttpGet("check-subscription")]
        public void CheckSubscription()
        {
            backgroundJob.AddOrUpdate("check-subscription", () => dominoJob.CheckSubscription(), Cron.Daily(0));
        }

        [HttpGet("clear-pending")]
        public void DeletePendingInvestment()
        {
            backgroundJobClient.ContinueJobWith("check-subscription", () => dominoJob.ClearPendingInvestments(),
                JobContinuationOptions.OnAnyFinishedState);
        }
    }
}