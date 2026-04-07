using System.ComponentModel.DataAnnotations;

namespace SmartOpsMonitoring.Api.DTOs.Requests;

public class CreateDeviceRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Location { get; set; } = string.Empty;

    public string? IpAddress { get; set; }
    public string? Tags { get; set; }
}

public class UpdateDeviceRequest
{
    [StringLength(100, MinimumLength = 1)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    [AllowedValues("Online", "Offline", "Warning", "Error", "Maintenance")]
    public string? Status { get; set; }

    public string? IpAddress { get; set; }
    public string? Tags { get; set; }
    public bool? IsActive { get; set; }
}
