using SmartOpsMonitoring.Domain.Enums;

namespace SmartOpsMonitoring.Domain.Entities;

/// <summary>
/// Represents a service or process node running on a monitored host.
/// </summary>
public class ServiceNode : BaseEntity
{
    /// <summary>Gets or sets the display name of the service node.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the type of service (e.g., "web", "database", "queue").</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the identifier of the host on which this node runs.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the current operational status of the service node.</summary>
    public ServiceNodeStatus Status { get; set; } = ServiceNodeStatus.Unknown;

    /// <summary>Gets or sets the port number the service listens on, if applicable.</summary>
    public int? Port { get; set; }

    /// <summary>Gets or sets the host entity this service node belongs to.</summary>
    public Host? Host { get; set; }
}
