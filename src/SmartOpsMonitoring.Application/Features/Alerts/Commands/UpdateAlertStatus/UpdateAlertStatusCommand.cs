namespace SmartOpsMonitoring.Application.Features.Alerts.Commands.UpdateAlertStatus;

/// <summary>
/// Command to update the lifecycle status of an existing alert.
/// </summary>
public class UpdateAlertStatusCommand : ICommand<AlertDto>
{
    /// <summary>Gets or sets the identifier of the alert to update.</summary>
    public Guid AlertId { get; set; }

    /// <summary>Gets or sets the new status string (e.g., "Acknowledged", "Resolved").</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets the identifier of the user performing the update, if applicable.</summary>
    public string? UserId { get; set; }
}
