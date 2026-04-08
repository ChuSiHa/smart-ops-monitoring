using Microsoft.Extensions.Logging;

namespace SmartOpsMonitoring.Infrastructure.Jobs;

/// <summary>
/// Hangfire background job that removes or transitions stale open alerts.
/// </summary>
public class StaleAlertCleanupJob
{
    private readonly ILogger<StaleAlertCleanupJob> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="StaleAlertCleanupJob"/>.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public StaleAlertCleanupJob(ILogger<StaleAlertCleanupJob> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes the stale alert cleanup logic.
    /// </summary>
    public Task ExecuteAsync()
    {
        _logger.LogInformation("Stale alert cleanup running at {Time}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
