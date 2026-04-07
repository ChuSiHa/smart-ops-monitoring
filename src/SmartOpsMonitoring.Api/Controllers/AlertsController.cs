using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.Services;

namespace SmartOpsMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    /// <summary>Get all alerts, optionally filtered by status and severity.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? severity,
        CancellationToken cancellationToken)
    {
        var alerts = await _alertService.GetAllAsync(status, severity, cancellationToken);
        return Ok(alerts);
    }

    /// <summary>Get an alert by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var alert = await _alertService.GetByIdAsync(id, cancellationToken);
        if (alert == null) return NotFound();
        return Ok(alert);
    }

    /// <summary>Create a new alert.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateAlertRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var alert = await _alertService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = alert.Id }, alert);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Update an alert's status (Acknowledge or Resolve).</summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAlertRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId() ?? 0;
        var alert = await _alertService.UpdateAsync(id, request, userId, cancellationToken);
        if (alert == null) return NotFound();
        return Ok(alert);
    }

    /// <summary>Delete an alert.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _alertService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var sub = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst("sub")?.Value;
        return int.TryParse(sub, out var id) ? id : null;
    }
}
