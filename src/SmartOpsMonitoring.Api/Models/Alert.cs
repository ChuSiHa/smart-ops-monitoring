namespace SmartOpsMonitoring.Api.Models;

public class Alert
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info";
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int? AcknowledgedByUserId { get; set; }

    public Device Device { get; set; } = null!;
}
