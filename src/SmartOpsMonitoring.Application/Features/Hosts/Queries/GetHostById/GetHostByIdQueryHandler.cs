using MediatR;
using SmartOpsMonitoring.Application.DTOs;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Application.Features.Hosts.Queries.GetHostById;

/// <summary>
/// Handles <see cref="GetHostByIdQuery"/> by retrieving a single host entity.
/// </summary>
public class GetHostByIdQueryHandler : IRequestHandler<GetHostByIdQuery, HostDto?>
{
    private readonly IHostRepository _hostRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetHostByIdQueryHandler"/>.
    /// </summary>
    /// <param name="hostRepository">The host repository.</param>
    public GetHostByIdQueryHandler(IHostRepository hostRepository)
    {
        _hostRepository = hostRepository;
    }

    /// <summary>
    /// Executes the query and returns the matching host as a DTO, or <c>null</c> if not found.
    /// </summary>
    /// <param name="request">The query containing the host identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="HostDto"/> or <c>null</c>.</returns>
    public async Task<HostDto?> Handle(GetHostByIdQuery request, CancellationToken cancellationToken)
    {
        var host = await _hostRepository.GetByIdAsync(request.Id, cancellationToken);
        if (host is null) return null;

        return new HostDto
        {
            Id = host.Id,
            Name = host.Name,
            IpAddress = host.IpAddress,
            OsType = host.OsType,
            Status = host.Status,
            Tags = host.Tags,
            CreatedAt = host.CreatedAt,
            UpdatedAt = host.UpdatedAt
        };
    }
}
