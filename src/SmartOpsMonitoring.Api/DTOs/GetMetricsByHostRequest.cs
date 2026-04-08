namespace SmartOpsMonitoring.Api.DTOs;

/// <summary>
/// Request DTO for querying metrics by host with optional filters.
/// Bound from query-string parameters.
/// </summary>
public class GetMetricsByHostRequest
{
    /// <summary>Gets or sets an optional metric type filter (e.g., "cpu_usage").</summary>
    public string? MetricType { get; set; }

    /// <summary>Gets or sets the optional start of the time range (UTC).</summary>
    public DateTime? From { get; set; }

    /// <summary>Gets or sets the optional end of the time range (UTC).</summary>
    public DateTime? To { get; set; }
}
