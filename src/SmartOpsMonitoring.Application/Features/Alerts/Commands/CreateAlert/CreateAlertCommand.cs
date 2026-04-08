namespace SmartOpsMonitoring.Application.Features.Alerts.Commands.CreateAlert;

/// <summary>
/// Command to create a new monitoring alert.
/// </summary>
public class CreateAlertCommand : ICommand<AlertDto>
{
    /// <summary>Gets or sets the host identifier associated with this alert.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the optional service node identifier.</summary>
    public Guid? ServiceNodeId { get; set; }

    /// <summary>Gets or sets the short title of the alert.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the detailed alert message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Gets or sets the severity as a string (e.g., "Info", "Warning", "Critical").</summary>
    public string Severity { get; set; } = string.Empty;
}
