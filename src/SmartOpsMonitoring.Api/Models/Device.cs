namespace SmartOpsMonitoring.Api.Models;

public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = "Offline";
    public string? IpAddress { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Metric> Metrics { get; set; } = new List<Metric>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
