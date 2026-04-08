using MediatR;
using SmartOpsMonitoring.Application.DTOs;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Application.Features.Hosts.Queries.GetHosts;

/// <summary>
/// Handles <see cref="GetHostsQuery"/> by returning all hosts as DTOs.
/// </summary>
public class GetHostsQueryHandler : IRequestHandler<GetHostsQuery, IEnumerable<HostDto>>
{
    private readonly IHostRepository _hostRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetHostsQueryHandler"/>.
    /// </summary>
    /// <param name="hostRepository">The host repository.</param>
    public GetHostsQueryHandler(IHostRepository hostRepository)
    {
        _hostRepository = hostRepository;
    }

    /// <summary>
    /// Executes the query and returns all hosts mapped to DTOs.
    /// </summary>
    /// <param name="request">The query (no parameters).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of <see cref="HostDto"/>.</returns>
    public async Task<IEnumerable<HostDto>> Handle(GetHostsQuery request, CancellationToken cancellationToken)
    {
        var hosts = await _hostRepository.GetAllAsync(cancellationToken);

        return hosts.Select(h => new HostDto
        {
            Id = h.Id,
            Name = h.Name,
            IpAddress = h.IpAddress,
            OsType = h.OsType,
            Status = h.Status,
            Tags = h.Tags,
            CreatedAt = h.CreatedAt,
            UpdatedAt = h.UpdatedAt
        });
    }
}
