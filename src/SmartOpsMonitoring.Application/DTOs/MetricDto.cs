namespace SmartOpsMonitoring.Application.DTOs;

/// <summary>Flat response DTO for a metric data point.</summary>
public class MetricDto
{
    /// <summary>Gets or sets the metric identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the host identifier.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the optional service node identifier.</summary>
    public Guid? ServiceNodeId { get; set; }

    /// <summary>Gets or sets the metric type name.</summary>
    public string MetricType { get; set; } = string.Empty;

    /// <summary>Gets or sets the numeric value.</summary>
    public double Value { get; set; }

    /// <summary>Gets or sets the unit of measurement.</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC timestamp when the metric was recorded.</summary>
    public DateTime Timestamp { get; set; }

    /// <summary>Gets or sets the key-value labels attached to this metric.</summary>
    public IDictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();
}
