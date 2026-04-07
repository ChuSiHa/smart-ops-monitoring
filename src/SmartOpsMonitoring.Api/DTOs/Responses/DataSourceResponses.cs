namespace SmartOpsMonitoring.Api.DTOs.Responses;

public class DataSourceResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? AdditionalConfig { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastTestedAt { get; set; }
    public bool? LastTestPassed { get; set; }
}
