using MediatR;
using SmartOpsMonitoring.Application.DTOs;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Application.Features.Alerts.Commands.UpdateAlertStatus;

/// <summary>
/// Handles <see cref="UpdateAlertStatusCommand"/> by updating an existing alert's status.
/// </summary>
public class UpdateAlertStatusCommandHandler : IRequestHandler<UpdateAlertStatusCommand, AlertDto>
{
    private readonly IAlertRepository _alertRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="UpdateAlertStatusCommandHandler"/>.
    /// </summary>
    /// <param name="alertRepository">The alert repository.</param>
    public UpdateAlertStatusCommandHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    /// <summary>
    /// Executes the command: locates the alert, transitions its status, and persists the update.
    /// </summary>
    /// <param name="request">The update status command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated <see cref="AlertDto"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the alert is not found.</exception>
    public async Task<AlertDto> Handle(UpdateAlertStatusCommand request, CancellationToken cancellationToken)
    {
        var alert = await _alertRepository.GetByIdAsync(request.AlertId, cancellationToken)
            ?? throw new KeyNotFoundException($"Alert {request.AlertId} not found.");

        var newStatus = Enum.Parse<AlertStatus>(request.Status, true);
        alert.Status = newStatus;
        alert.UpdatedAt = DateTime.UtcNow;

        if (newStatus == AlertStatus.Acknowledged)
        {
            alert.AcknowledgedAt = DateTime.UtcNow;
            alert.AcknowledgedByUserId = request.UserId;
        }
        else if (newStatus == AlertStatus.Resolved)
        {
            alert.ResolvedAt = DateTime.UtcNow;
        }

        await _alertRepository.UpdateAsync(alert, cancellationToken);

        return new AlertDto
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
}
