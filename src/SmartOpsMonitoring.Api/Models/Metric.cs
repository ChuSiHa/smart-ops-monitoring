namespace SmartOpsMonitoring.Api.Models;

public class Metric
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string MetricType { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Labels { get; set; }

    public Device Device { get; set; } = null!;
}
