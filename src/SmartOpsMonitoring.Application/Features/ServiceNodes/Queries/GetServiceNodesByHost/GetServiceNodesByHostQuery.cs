using MediatR;
using SmartOpsMonitoring.Application.DTOs;

namespace SmartOpsMonitoring.Application.Features.ServiceNodes.Queries.GetServiceNodesByHost;

/// <summary>
/// Query to retrieve all service nodes belonging to a specific host.
/// </summary>
public class GetServiceNodesByHostQuery : IRequest<IEnumerable<ServiceNodeDto>>
{
    /// <summary>Gets or sets the host identifier.</summary>
    public Guid HostId { get; set; }
}
