using SmartOpsMonitoring.Domain.Enums;

namespace SmartOpsMonitoring.Domain.Entities;

/// <summary>
/// Represents a physical or virtual host being monitored.
/// </summary>
public class Host : BaseEntity
{
    /// <summary>Gets or sets the display name of the host.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the IP address of the host.</summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>Gets or sets the operating system type running on the host.</summary>
    public string OsType { get; set; } = string.Empty;

    /// <summary>Gets or sets the current operational status of the host.</summary>
    public HostStatus Status { get; set; } = HostStatus.Unknown;

    /// <summary>Gets or sets the collection of tags associated with the host.</summary>
    public ICollection<string> Tags { get; set; } = new List<string>();

    /// <summary>Gets or sets the service nodes running on this host.</summary>
    public ICollection<ServiceNode> ServiceNodes { get; set; } = new List<ServiceNode>();
}
