namespace SmartOpsMonitoring.Application.DTOs;

/// <summary>Input DTO for updating the lifecycle status of an alert.</summary>
public class UpdateAlertStatusDto
{
    /// <summary>Gets or sets the identifier of the alert to update.</summary>
    public Guid AlertId { get; set; }

    /// <summary>Gets or sets the new status as a string (e.g., "Acknowledged", "Resolved").</summary>
    public string Status { get; set; } = string.Empty;
}
