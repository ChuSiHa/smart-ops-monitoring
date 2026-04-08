namespace SmartOpsMonitoring.Domain.Enums;

/// <summary>
/// Represents the operational status of a service node.
/// </summary>
public enum ServiceNodeStatus
{
    /// <summary>Status is not yet determined.</summary>
    Unknown = 0,

    /// <summary>Service node is running normally.</summary>
    Running = 1,

    /// <summary>Service node has been stopped.</summary>
    Stopped = 2,

    /// <summary>Service node is in an error state.</summary>
    Error = 3
}
