using System.ComponentModel.DataAnnotations;

namespace SmartOpsMonitoring.Api.DTOs.Requests;

public class CreateAlertRequest
{
    [Required]
    public int DeviceId { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string Message { get; set; } = string.Empty;

    [AllowedValues("Info", "Warning", "Critical")]
    public string Severity { get; set; } = "Info";
}

public class UpdateAlertRequest
{
    [AllowedValues("Open", "Acknowledged", "Resolved")]
    public string? Status { get; set; }
}
