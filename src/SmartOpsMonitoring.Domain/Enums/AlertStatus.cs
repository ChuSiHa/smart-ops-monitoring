namespace SmartOpsMonitoring.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of an alert.
/// </summary>
public enum AlertStatus
{
    /// <summary>Alert is open and unacknowledged.</summary>
    Open = 0,

    /// <summary>Alert has been acknowledged by an operator.</summary>
    Acknowledged = 1,

    /// <summary>Alert has been resolved.</summary>
    Resolved = 2
}
