using Hangfire;
using Tracer.Api.BackgroundServices;

namespace Tracer.Api;

public static class HangfireJobsConfig
{
    public static void ScheduleRecurringJobs()
    {
        RecurringJob.AddOrUpdate<ProcessOutboxMessagesJob>(
            "ProcessOutboxMessages",
            job => job.ExecuteAsync(CancellationToken.None),
            "*/15 * * * * *"); // Every 15 seconds

        // Monthly asset valuation run
        RecurringJob.AddOrUpdate<CalculateAssetValuationsJob>(
            "CalculateAssetValuations",
            job => job.ExecuteAsync(CancellationToken.None),
            Cron.Monthly());
    }
}
