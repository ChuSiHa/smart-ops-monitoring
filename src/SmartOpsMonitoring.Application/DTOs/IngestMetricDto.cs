namespace SmartOpsMonitoring.Application.DTOs;

/// <summary>
/// Input DTO for ingesting a new metric data point.
/// </summary>
public class IngestMetricDto
{
    /// <summary>Gets or sets the host identifier the metric originates from.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the optional service node identifier.</summary>
    public Guid? ServiceNodeId { get; set; }

    /// <summary>Gets or sets the metric type name (e.g., "cpu_usage").</summary>
    public string MetricType { get; set; } = string.Empty;

    /// <summary>Gets or sets the numeric metric value.</summary>
    public double Value { get; set; }

    /// <summary>Gets or sets the unit of measurement.</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional UTC timestamp; defaults to now if not provided.</summary>
    public DateTime? Timestamp { get; set; }

    /// <summary>Gets or sets optional key-value labels for the metric.</summary>
    public IDictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();
}
