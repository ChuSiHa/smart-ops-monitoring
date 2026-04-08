using MediatR;
using SmartOpsMonitoring.Application.DTOs;

namespace SmartOpsMonitoring.Application.Features.Alerts.Queries.GetAlerts;

/// <summary>
/// Query to retrieve alerts with optional filters.
/// </summary>
public class GetAlertsQuery : IRequest<IEnumerable<AlertDto>>
{
    /// <summary>Gets or sets an optional host identifier filter.</summary>
    public Guid? HostId { get; set; }

    /// <summary>Gets or sets an optional alert status filter string.</summary>
    public string? Status { get; set; }

    /// <summary>Gets or sets an optional alert severity filter string.</summary>
    public string? Severity { get; set; }
}
