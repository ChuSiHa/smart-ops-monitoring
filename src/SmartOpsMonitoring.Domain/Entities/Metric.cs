namespace SmartOpsMonitoring.Domain.Entities;

/// <summary>
/// Represents a single telemetry data point collected from a host or service node.
/// </summary>
public class Metric : BaseEntity
{
    /// <summary>Gets or sets the identifier of the host this metric originates from.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the optional identifier of the service node this metric relates to.</summary>
    public Guid? ServiceNodeId { get; set; }

    /// <summary>Gets or sets the type or name of the metric (e.g., "cpu_usage", "memory_bytes").</summary>
    public string MetricType { get; set; } = string.Empty;

    /// <summary>Gets or sets the numeric value of the metric.</summary>
    public double Value { get; set; }

    /// <summary>Gets or sets the unit of measurement (e.g., "percent", "bytes").</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC timestamp at which the metric was recorded.</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets additional key-value labels for this metric.</summary>
    public IDictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();
}
