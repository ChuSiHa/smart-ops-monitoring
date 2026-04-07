using System.ComponentModel.DataAnnotations;

namespace SmartOpsMonitoring.Api.DTOs.Requests;

public class IngestMetricRequest
{
    [Required]
    public int DeviceId { get; set; }

    [Required, StringLength(100)]
    public string MetricType { get; set; } = string.Empty;

    [Required]
    public double Value { get; set; }

    [Required, StringLength(20)]
    public string Unit { get; set; } = string.Empty;

    public DateTime? Timestamp { get; set; }
    public string? Labels { get; set; }
}

public class MetricQueryRequest
{
    public int? DeviceId { get; set; }
    public string? MetricType { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int PageSize { get; set; } = 100;
    public int Page { get; set; } = 1;
}
