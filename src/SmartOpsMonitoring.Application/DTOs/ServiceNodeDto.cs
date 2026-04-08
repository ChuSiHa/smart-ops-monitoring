namespace SmartOpsMonitoring.Application.DTOs;

/// <summary>
/// Flat response DTO for a service node.
/// </summary>
public class ServiceNodeDto
{
    /// <summary>Gets or sets the service node identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the service node name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the service type.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the owning host identifier.</summary>
    public Guid HostId { get; set; }

    /// <summary>Gets or sets the current status.</summary>
    public ServiceNodeStatus Status { get; set; }

    /// <summary>Gets or sets the port the service listens on.</summary>
    public int? Port { get; set; }

    /// <summary>Gets or sets when the record was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets when the record was last updated.</summary>
    public DateTime UpdatedAt { get; set; }
}
