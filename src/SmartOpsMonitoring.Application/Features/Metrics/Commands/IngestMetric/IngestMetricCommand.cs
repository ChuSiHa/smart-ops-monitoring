using MediatR;
using SmartOpsMonitoring.Application.DTOs;

namespace SmartOpsMonitoring.Application.Features.Metrics.Commands.IngestMetric;

/// <summary>
/// Command to ingest a new metric data point into the system.
/// </summary>
public class IngestMetricCommand : IRequest<MetricDto>
{
    /// <summary>Gets or sets the host identifier the metric originates from.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the optional service node identifier.</summary>
    public Guid? ServiceNodeId { get; set; }

    /// <summary>Gets or sets the metric type name.</summary>
    public string MetricType { get; set; } = string.Empty;

    /// <summary>Gets or sets the numeric metric value.</summary>
    public double Value { get; set; }

    /// <summary>Gets or sets the unit of measurement.</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional UTC timestamp; defaults to <see cref="DateTime.UtcNow"/> when absent.</summary>
    public DateTime? Timestamp { get; set; }

    /// <summary>Gets or sets optional key-value labels for the metric.</summary>
    public IDictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();
}
