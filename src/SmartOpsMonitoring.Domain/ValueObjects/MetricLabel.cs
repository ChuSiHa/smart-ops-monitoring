namespace SmartOpsMonitoring.Domain.ValueObjects;

/// <summary>
/// Value object representing a key-value label attached to a metric.
/// </summary>
/// <param name="Key">The label key.</param>
/// <param name="Value">The label value.</param>
public record MetricLabel(string Key, string Value);
