using Microsoft.Extensions.Logging;

namespace SmartOpsMonitoring.Infrastructure.Jobs;

/// <summary>
/// Hangfire background job that polls registered hosts and service nodes for health status.
/// </summary>
public class HealthCheckPollingJob
{
    private readonly ILogger<HealthCheckPollingJob> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="HealthCheckPollingJob"/>.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public HealthCheckPollingJob(ILogger<HealthCheckPollingJob> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes the health-check polling logic.
    /// </summary>
    public Task ExecuteAsync()
    {
        _logger.LogInformation("Health check polling running at {Time}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
