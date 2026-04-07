using System.ComponentModel.DataAnnotations;

namespace SmartOpsMonitoring.Api.DTOs.Requests;

public class CreateDataSourceRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    public string? AdditionalConfig { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateDataSourceRequest
{
    [StringLength(100, MinimumLength = 1)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    public string? ConnectionString { get; set; }
    public string? AdditionalConfig { get; set; }
    public bool? IsActive { get; set; }
}
