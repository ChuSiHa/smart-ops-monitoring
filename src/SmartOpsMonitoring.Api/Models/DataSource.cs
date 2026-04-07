namespace SmartOpsMonitoring.Api.Models;

public class DataSource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string? AdditionalConfig { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastTestedAt { get; set; }
    public bool? LastTestPassed { get; set; }
}
