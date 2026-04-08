namespace SmartOpsMonitoring.Application.Features.Alerts.Commands.CreateAlert;

/// <summary>
/// Handles <see cref="CreateAlertCommand"/> by persisting an alert and publishing a domain event.
/// </summary>
public class CreateAlertCommandHandler : ICommandHandler<CreateAlertCommand, AlertDto>
{
    private readonly IAlertRepository _alertRepository;
    private readonly IPublisher _publisher;

    /// <summary>
    /// Initialises a new instance of <see cref="CreateAlertCommandHandler"/>.
    /// </summary>
    /// <param name="alertRepository">The alert repository.</param>
    /// <param name="publisher">The MediatR publisher.</param>
    public CreateAlertCommandHandler(IAlertRepository alertRepository, IPublisher publisher)
    {
        _alertRepository = alertRepository;
        _publisher = publisher;
    }

    /// <summary>
    /// Executes the command: creates and persists an <see cref="Alert"/>, then publishes
    /// an <see cref="AlertCreatedEvent"/>.
    /// </summary>
    /// <param name="request">The create alert command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="AlertDto"/> representing the created alert.</returns>
    public async Task<AlertDto> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
    {
        var severity = Enum.Parse<AlertSeverity>(request.Severity, true);

        var alert = new Alert
        {
            HostId = request.HostId,
            ServiceNodeId = request.ServiceNodeId,
            Title = request.Title,
            Message = request.Message,
            Severity = severity,
            Status = AlertStatus.Open
        };

        await _alertRepository.AddAsync(alert, cancellationToken);

        // Publish domain event so subscribers (e.g. SignalR hub, notification service) can react.
        await _publisher.Publish(
            new AlertCreatedEvent(alert.Id, alert.Severity, alert.HostId),
            cancellationToken);

        return MapToDto(alert);
    }

    /// <summary>
    /// Maps an <see cref="Alert"/> entity to an <see cref="AlertDto"/>.
    /// </summary>
    /// <param name="alert">The alert entity to map.</param>
    /// <returns>The mapped <see cref="AlertDto"/>.</returns>
    private static AlertDto MapToDto(Alert alert) => new()
    {
        Id = alert.Id,
        HostId = alert.HostId,
        ServiceNodeId = alert.ServiceNodeId,
        Title = alert.Title,
        Message = alert.Message,
        Severity = alert.Severity,
        Status = alert.Status,
        AcknowledgedAt = alert.AcknowledgedAt,
        ResolvedAt = alert.ResolvedAt,
        AcknowledgedByUserId = alert.AcknowledgedByUserId,
        CreatedAt = alert.CreatedAt
    };
}
