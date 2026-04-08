using MediatR;
using SmartOpsMonitoring.Application.DTOs;

namespace SmartOpsMonitoring.Application.Features.Hosts.Queries.GetHostById;

/// <summary>
/// Query to retrieve a single host by its identifier.
/// </summary>
public class GetHostByIdQuery : IRequest<HostDto?>
{
    /// <summary>Gets or sets the host identifier.</summary>
    public Guid Id { get; set; }
}
