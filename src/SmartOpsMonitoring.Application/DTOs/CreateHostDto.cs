namespace SmartOpsMonitoring.Application.DTOs;

/// <summary>
/// Input DTO for registering a new host.
/// </summary>
public class CreateHostDto
{
    /// <summary>Gets or sets the display name for the new host.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the IP address of the host.</summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>Gets or sets the operating system type (e.g., "Linux", "Windows").</summary>
    public string OsType { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional set of tags to associate with the host.</summary>
    public ICollection<string> Tags { get; set; } = new List<string>();
}
