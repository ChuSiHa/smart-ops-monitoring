namespace SmartOpsMonitoring.Application.Features.ServiceNodes.Commands.CreateServiceNode;

/// <summary>
/// Command to register a new service node on a host.
/// </summary>
public class CreateServiceNodeCommand : ICommand<ServiceNodeDto>
{
    /// <summary>Gets or sets the display name of the service node.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the service type (e.g., "nginx", "postgres").</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the identifier of the owning host.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the optional port the service listens on.</summary>
    public int? Port { get; set; }
}
