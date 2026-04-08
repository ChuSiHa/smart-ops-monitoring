namespace SmartOpsMonitoring.Application.DTOs;

/// <summary>
/// Flat response DTO for a monitored host.
/// </summary>
public class HostDto
{
    /// <summary>Gets or sets the host identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the host name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the IP address.</summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>Gets or sets the operating system type.</summary>
    public string OsType { get; set; } = string.Empty;

    /// <summary>Gets or sets the current status.</summary>
    public HostStatus Status { get; set; }

    /// <summary>Gets or sets the tags associated with the host.</summary>
    public ICollection<string> Tags { get; set; } = new List<string>();

    /// <summary>Gets or sets when the record was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets when the record was last updated.</summary>
    public DateTime UpdatedAt { get; set; }
}
