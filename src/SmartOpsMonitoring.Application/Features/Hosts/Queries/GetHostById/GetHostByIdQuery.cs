namespace SmartOpsMonitoring.Application.Features.Hosts.Queries.GetHostById;

/// <summary>
/// Query to retrieve a single host by its identifier.
/// </summary>
public class GetHostByIdQuery : IQuery<HostDto?>
{
    /// <summary>Gets or sets the host identifier.</summary>
    public Guid Id { get; set; }
}
