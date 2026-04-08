namespace SmartOpsMonitoring.Application.DTOs;

/// <summary>
/// Flat response DTO for an alert.
/// </summary>
public class AlertDto
{
    /// <summary>Gets or sets the alert identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the host identifier.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the optional service node identifier.</summary>
    public Guid? ServiceNodeId { get; set; }

    /// <summary>Gets or sets the alert title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the alert message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Gets or sets the alert severity.</summary>
    public AlertSeverity Severity { get; set; }

    /// <summary>Gets or sets the alert lifecycle status.</summary>
    public AlertStatus Status { get; set; }

    /// <summary>Gets or sets when the alert was acknowledged.</summary>
    public DateTime? AcknowledgedAt { get; set; }

    /// <summary>Gets or sets when the alert was resolved.</summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>Gets or sets the user ID who acknowledged the alert.</summary>
    public string? AcknowledgedByUserId { get; set; }

    /// <summary>Gets or sets when the record was created.</summary>
    public DateTime CreatedAt { get; set; }
}
