namespace SmartOpsMonitoring.Application.Features.Hosts.Commands.CreateHost;

/// <summary>
/// Handles <see cref="CreateHostCommand"/> by creating and persisting a new host entity.
/// </summary>
public class CreateHostCommandHandler : ICommandHandler<CreateHostCommand, HostDto>
{
    private readonly IHostRepository _hostRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="CreateHostCommandHandler"/>.
    /// </summary>
    /// <param name="hostRepository">The host repository.</param>
    public CreateHostCommandHandler(IHostRepository hostRepository)
    {
        _hostRepository = hostRepository;
    }

    /// <summary>
    /// Executes the command: creates and persists a new <see cref="Domain.Entities.Host"/>.
    /// </summary>
    /// <param name="request">The create host command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="HostDto"/> representing the newly created host.</returns>
    public async Task<HostDto> Handle(CreateHostCommand request, CancellationToken cancellationToken)
    {
        var host = new Domain.Entities.Host
        {
            Name = request.Name,
            IpAddress = request.IpAddress,
            OsType = request.OsType,
            Tags = request.Tags
        };

        await _hostRepository.AddAsync(host, cancellationToken);

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
