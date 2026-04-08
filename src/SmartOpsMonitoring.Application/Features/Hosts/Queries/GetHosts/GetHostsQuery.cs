using MediatR;
using SmartOpsMonitoring.Application.DTOs;

namespace SmartOpsMonitoring.Application.Features.Hosts.Queries.GetHosts;

/// <summary>
/// Query to retrieve all registered hosts.
/// </summary>
public class GetHostsQuery : IRequest<IEnumerable<HostDto>>
{
}
