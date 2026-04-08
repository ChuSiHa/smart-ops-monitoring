namespace SmartOpsMonitoring.Application.DTOs;

/// <summary>Input DTO for registering a new service node.</summary>
public class CreateServiceNodeDto
{
    /// <summary>Gets or sets the display name for the new service node.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the service type (e.g., "nginx", "postgres").</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the identifier of the host this service node belongs to.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the optional port the service listens on.</summary>
    public int? Port { get; set; }
}
