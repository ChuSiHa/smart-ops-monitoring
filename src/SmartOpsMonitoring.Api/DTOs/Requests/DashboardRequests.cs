using System.ComponentModel.DataAnnotations;

namespace SmartOpsMonitoring.Api.DTOs.Requests;

public class CreateDashboardRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public string? ConfigJson { get; set; }
    public bool IsDefault { get; set; } = false;
}

public class UpdateDashboardRequest
{
    [StringLength(100, MinimumLength = 1)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public string? ConfigJson { get; set; }
    public bool? IsDefault { get; set; }
}
