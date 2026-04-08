namespace SmartOpsMonitoring.Application.Features.Hosts.Commands.CreateHost;

/// <summary>
/// Command to register a new monitored host.
/// </summary>
public class CreateHostCommand : ICommand<HostDto>
{
    /// <summary>Gets or sets the display name for the new host.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the IP address of the host.</summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>Gets or sets the operating system type (e.g., "Linux", "Windows").</summary>
    public string OsType { get; set; } = string.Empty;

    /// <summary>Gets or sets tags to associate with the new host.</summary>
    public ICollection<string> Tags { get; set; } = new List<string>();
}
