namespace SmartOpsMonitoring.Application.Features.ServiceNodes.Commands.CreateServiceNode;

/// <summary>
/// Handles <see cref="CreateServiceNodeCommand"/> by persisting a new service node.
/// </summary>
public class CreateServiceNodeCommandHandler : ICommandHandler<CreateServiceNodeCommand, ServiceNodeDto>
{
    private readonly IServiceNodeRepository _serviceNodeRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="CreateServiceNodeCommandHandler"/>.
    /// </summary>
    /// <param name="serviceNodeRepository">The service node repository.</param>
    public CreateServiceNodeCommandHandler(IServiceNodeRepository serviceNodeRepository)
    {
        _serviceNodeRepository = serviceNodeRepository;
    }

    /// <summary>
    /// Executes the command: creates and persists a new <see cref="ServiceNode"/>.
    /// </summary>
    /// <param name="request">The create service node command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="ServiceNodeDto"/> representing the newly created service node.</returns>
    public async Task<ServiceNodeDto> Handle(CreateServiceNodeCommand request, CancellationToken cancellationToken)
    {
        var node = request.Adapt<ServiceNode>();
        await _serviceNodeRepository.AddAsync(node, cancellationToken);
        return node.Adapt<ServiceNodeDto>();
    }
}
