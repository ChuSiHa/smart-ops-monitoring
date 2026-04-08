namespace SmartOpsMonitoring.Domain.Enums;

/// <summary>
/// Represents the operational status of a monitored host.
/// </summary>
public enum HostStatus
{
    /// <summary>Status is not yet determined.</summary>
    Unknown = 0,

    /// <summary>Host is reachable and operational.</summary>
    Online = 1,

    /// <summary>Host is unreachable.</summary>
    Offline = 2,

    /// <summary>Host is undergoing planned maintenance.</summary>
    Maintenance = 3
}
