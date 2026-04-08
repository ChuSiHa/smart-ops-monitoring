using SmartOpsMonitoring.Domain.Enums;

namespace SmartOpsMonitoring.Domain.Events;

/// <summary>
/// Domain event raised when a new alert is created.
/// </summary>
public class AlertCreatedEvent : IDomainEvent
{
    /// <summary>Gets the identifier of the newly created alert.</summary>
    public Guid AlertId { get; }

    /// <summary>Gets the severity level of the alert.</summary>
    public AlertSeverity Severity { get; }

    /// <summary>Gets the identifier of the host associated with the alert.</summary>
    public Guid HostId { get; }

    /// <summary>
    /// Initialises a new instance of <see cref="AlertCreatedEvent"/>.
    /// </summary>
    /// <param name="alertId">The identifier of the alert.</param>
    /// <param name="severity">The alert severity.</param>
    /// <param name="hostId">The host identifier.</param>
    public AlertCreatedEvent(Guid alertId, AlertSeverity severity, Guid hostId)
    {
        AlertId = alertId;
        Severity = severity;
        HostId = hostId;
    }
}
