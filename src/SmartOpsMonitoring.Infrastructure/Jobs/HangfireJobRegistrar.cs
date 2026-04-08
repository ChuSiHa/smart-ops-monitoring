namespace SmartOpsMonitoring.Infrastructure.Jobs;

/// <summary>
/// Registers recurring Hangfire background jobs with their cron schedules.
/// </summary>
public static class HangfireJobRegistrar
{
    /// <summary>
    /// Registers all recurring jobs with the Hangfire recurring job manager.
    /// </summary>
    /// <param name="recurringJobManager">The Hangfire recurring job manager.</param>
    public static void RegisterRecurringJobs(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<MetricAggregationJob>(
            "metric-aggregation",
            job => job.ExecuteAsync(),
            Cron.Minutely());

        recurringJobManager.AddOrUpdate<StaleAlertCleanupJob>(
            "stale-alert-cleanup",
            job => job.ExecuteAsync(),
            Cron.Daily());

        recurringJobManager.AddOrUpdate<HealthCheckPollingJob>(
            "health-check-polling",
            job => job.ExecuteAsync(),
            "*/5 * * * *"); // every 5 minutes
    }
}
