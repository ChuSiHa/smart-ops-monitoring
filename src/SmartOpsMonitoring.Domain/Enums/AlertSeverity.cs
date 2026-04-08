namespace SmartOpsMonitoring.Domain.Enums;

/// <summary>
/// Indicates the severity level of an alert.
/// </summary>
public enum AlertSeverity
{
    /// <summary>Informational alert requiring no immediate action.</summary>
    Info = 0,

    /// <summary>Warning alert that may require attention.</summary>
    Warning = 1,

    /// <summary>Critical alert requiring immediate action.</summary>
    Critical = 2
}
