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
        public DominoJobs(IRecurringJobManager _backgroundJob, IBackgroundJobClient _backgroundJobClient, IDominoJob _dominoJob)
        {
            backgroundJob = _backgroundJob;
            backgroundJobClient = _backgroundJobClient;
            dominoJob = _dominoJob;
        }

        [HttpGet("complete-pairing")]
        public void CompletePairing()
        {
            backgroundJob.AddOrUpdate("complete-pairing", () => dominoJob.PerformPairInvestment(), Cron.Minutely);
        }

        [HttpGet("incomplete-pairing")]
        public void UncompletedPairing()
        {
            backgroundJobClient.ContinueJobWith("complete-pairing", () => dominoJob.CheckUnclosedPairing(), JobContinuationOptions.OnlyOnSucceededState);
        }
    }
}