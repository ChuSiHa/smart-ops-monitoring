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
        var host = request.Adapt<Domain.Entities.Host>();
        await _hostRepository.AddAsync(host, cancellationToken);
        return host.Adapt<HostDto>();
    }
}
