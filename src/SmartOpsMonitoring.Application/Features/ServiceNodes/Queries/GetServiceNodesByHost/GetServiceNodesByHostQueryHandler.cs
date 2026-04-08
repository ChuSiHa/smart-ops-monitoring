using MediatR;
using SmartOpsMonitoring.Application.DTOs;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Application.Features.ServiceNodes.Queries.GetServiceNodesByHost;

/// <summary>
/// Handles <see cref="GetServiceNodesByHostQuery"/> by retrieving service nodes for a host.
/// </summary>
public class GetServiceNodesByHostQueryHandler : IRequestHandler<GetServiceNodesByHostQuery, IEnumerable<ServiceNodeDto>>
{
    private readonly IServiceNodeRepository _serviceNodeRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetServiceNodesByHostQueryHandler"/>.
    /// </summary>
    /// <param name="serviceNodeRepository">The service node repository.</param>
    public GetServiceNodesByHostQueryHandler(IServiceNodeRepository serviceNodeRepository)
    {
        _serviceNodeRepository = serviceNodeRepository;
    }

    /// <summary>
    /// Executes the query and returns service nodes for the specified host.
    /// </summary>
    /// <param name="request">The query containing the host identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of <see cref="ServiceNodeDto"/>.</returns>
    public async Task<IEnumerable<ServiceNodeDto>> Handle(GetServiceNodesByHostQuery request, CancellationToken cancellationToken)
    {
        var nodes = await _serviceNodeRepository.GetByHostIdAsync(request.HostId, cancellationToken);

        return nodes.Select(n => new ServiceNodeDto
        {
            Id = n.Id,
            Name = n.Name,
            Type = n.Type,
            HostId = n.HostId,
            Status = n.Status,
            Port = n.Port,
            CreatedAt = n.CreatedAt,
            UpdatedAt = n.UpdatedAt
        });
    }
}
