using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOpsMonitoring.Application.Features.Alerts.Commands.CreateAlert;
using SmartOpsMonitoring.Application.Features.Alerts.Commands.UpdateAlertStatus;
using SmartOpsMonitoring.Application.Features.Alerts.Queries.GetAlerts;

namespace SmartOpsMonitoring.Api.Controllers;

/// <summary>
/// Exposes endpoints for creating, querying, and updating monitoring alerts.
/// </summary>
[Authorize]
public class AlertsController : BaseApiController
{
    /// <summary>
    /// Initialises a new instance of <see cref="AlertsController"/>.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    public AlertsController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Returns alerts, optionally filtered by host, status, or severity.
    /// </summary>
    /// <param name="hostId">Optional host identifier filter.</param>
    /// <param name="status">Optional status string filter.</param>
    /// <param name="severity">Optional severity string filter.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? hostId,
        [FromQuery] string? status,
        [FromQuery] string? severity,
        CancellationToken ct)
        => Ok(await Sender.Send(new GetAlertsQuery { HostId = hostId, Status = status, Severity = severity }, ct));

    /// <summary>
    /// Creates a new monitoring alert.
    /// </summary>
    /// <param name="command">The create alert command.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAlertCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Updates the status of an existing alert.
    /// </summary>
    /// <param name="id">The alert identifier.</param>
    /// <param name="command">The update status command.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id, [FromBody] UpdateAlertStatusCommand command, CancellationToken ct)
    {
        command.AlertId = id;
        command.UserId = User.Identity?.Name;
        var result = await Sender.Send(command, ct);
        return Ok(result);
    }
}
