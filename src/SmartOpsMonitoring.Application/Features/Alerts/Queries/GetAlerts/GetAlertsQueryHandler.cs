using MediatR;
using SmartOpsMonitoring.Application.DTOs;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Application.Features.Alerts.Queries.GetAlerts;

/// <summary>
/// Handles <see cref="GetAlertsQuery"/> by querying the alert repository with optional filters.
/// </summary>
public class GetAlertsQueryHandler : IRequestHandler<GetAlertsQuery, IEnumerable<AlertDto>>
{
    private readonly IAlertRepository _alertRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetAlertsQueryHandler"/>.
    /// </summary>
    /// <param name="alertRepository">The alert repository.</param>
    public GetAlertsQueryHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    /// <summary>
    /// Executes the query and returns matching alerts mapped to DTOs.
    /// </summary>
    /// <param name="request">The query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of <see cref="AlertDto"/>.</returns>
    public async Task<IEnumerable<AlertDto>> Handle(GetAlertsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Alert> alerts;

        if (request.HostId.HasValue)
        {
            alerts = await _alertRepository.GetByHostIdAsync(request.HostId.Value, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.Severity) && Enum.TryParse<AlertSeverity>(request.Severity, true, out var sev))
        {
            alerts = await _alertRepository.GetBySeverityAsync(sev, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.Status) &&
                 Enum.TryParse<AlertStatus>(request.Status, true, out var stat) &&
                 stat == AlertStatus.Open)
        {
            alerts = await _alertRepository.GetOpenAlertsAsync(cancellationToken);
        }
        else
        {
            alerts = await _alertRepository.GetAllAsync(cancellationToken);
        }

        // Apply remaining filters in memory
        if (request.HostId.HasValue)
            alerts = alerts.Where(a => a.HostId == request.HostId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<AlertStatus>(request.Status, true, out var statusFilter))
            alerts = alerts.Where(a => a.Status == statusFilter);

        if (!string.IsNullOrWhiteSpace(request.Severity) && Enum.TryParse<AlertSeverity>(request.Severity, true, out var severityFilter))
            alerts = alerts.Where(a => a.Severity == severityFilter);

        return alerts.Select(a => new AlertDto
        {
            Id = a.Id,
            HostId = a.HostId,
            ServiceNodeId = a.ServiceNodeId,
            Title = a.Title,
            Message = a.Message,
            Severity = a.Severity,
            Status = a.Status,
            AcknowledgedAt = a.AcknowledgedAt,
            ResolvedAt = a.ResolvedAt,
            AcknowledgedByUserId = a.AcknowledgedByUserId,
            CreatedAt = a.CreatedAt
        });
    }
}
