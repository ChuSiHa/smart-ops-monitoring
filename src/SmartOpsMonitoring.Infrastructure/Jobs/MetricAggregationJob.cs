namespace SmartOpsMonitoring.Infrastructure.Jobs;

/// <summary>
/// Hangfire background job that aggregates metric data on a scheduled basis.
/// </summary>
public class MetricAggregationJob
{
    private readonly ILogger<MetricAggregationJob> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="MetricAggregationJob"/>.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public MetricAggregationJob(ILogger<MetricAggregationJob> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes the metric aggregation logic.
    /// </summary>
    public Task ExecuteAsync()
    {
        _logger.LogInformation("Metric aggregation running at {Time}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
