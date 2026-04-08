using SmartOpsMonitoring.Domain.Enums;

namespace SmartOpsMonitoring.Domain.Entities;

/// <summary>
/// Represents a monitoring alert raised against a host or service node.
/// </summary>
public class Alert : BaseEntity
{
    /// <summary>Gets or sets the identifier of the host this alert pertains to.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the optional identifier of the service node this alert relates to.</summary>
    public Guid? ServiceNodeId { get; set; }

    /// <summary>Gets or sets the short title of the alert.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the detailed message describing the alert.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Gets or sets the severity level of the alert.</summary>
    public AlertSeverity Severity { get; set; } = AlertSeverity.Info;

    /// <summary>Gets or sets the current lifecycle status of the alert.</summary>
    public AlertStatus Status { get; set; } = AlertStatus.Open;

    /// <summary>Gets or sets the UTC timestamp when this alert was acknowledged, if applicable.</summary>
    public DateTime? AcknowledgedAt { get; set; }

    /// <summary>Gets or sets the UTC timestamp when this alert was resolved, if applicable.</summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>Gets or sets the identifier of the user who acknowledged this alert, if applicable.</summary>
    public string? AcknowledgedByUserId { get; set; }
}
