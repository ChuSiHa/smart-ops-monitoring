namespace SmartOpsMonitoring.Api.DTOs.Responses;

public class MetricResponse
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string MetricType { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Labels { get; set; }
}

public class PagedMetricResponse
{
    public IEnumerable<MetricResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
