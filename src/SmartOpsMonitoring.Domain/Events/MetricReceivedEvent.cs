namespace SmartOpsMonitoring.Domain.Events;

/// <summary>
/// Domain event raised when a new metric data point is received.
/// </summary>
public class MetricReceivedEvent : IDomainEvent
{
    /// <summary>Gets the identifier of the newly ingested metric.</summary>
    public Guid MetricId { get; }

    /// <summary>Gets the identifier of the host the metric belongs to.</summary>
    public Guid HostId { get; }

    /// <summary>Gets the type/name of the metric (e.g., "cpu_usage").</summary>
    public string MetricType { get; }

    /// <summary>Gets the numeric value of the metric.</summary>
    public double Value { get; }

    /// <summary>
    /// Initialises a new instance of <see cref="MetricReceivedEvent"/>.
    /// </summary>
    /// <param name="metricId">The metric identifier.</param>
    /// <param name="hostId">The host identifier.</param>
    /// <param name="metricType">The metric type string.</param>
    /// <param name="value">The numeric value.</param>
    public MetricReceivedEvent(Guid metricId, Guid hostId, string metricType, double value)
    {
        MetricId = metricId;
        HostId = hostId;
        MetricType = metricType;
        Value = value;
    }
}
