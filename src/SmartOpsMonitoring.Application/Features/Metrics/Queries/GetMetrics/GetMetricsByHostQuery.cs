namespace SmartOpsMonitoring.Application.Features.Metrics.Queries.GetMetrics;

/// <summary>
/// Query to retrieve metrics for a specific host, optionally filtered by type and time range.
/// </summary>
public class GetMetricsByHostQuery : IQuery<IEnumerable<MetricDto>>
{
    /// <summary>Gets or sets the host identifier.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets an optional metric type filter.</summary>
    public string? MetricType { get; set; }

    /// <summary>Gets or sets the optional start of the time range.</summary>
    public DateTime? From { get; set; }

    /// <summary>Gets or sets the optional end of the time range.</summary>
    public DateTime? To { get; set; }
}
