using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.Services;

namespace SmartOpsMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardsController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardsController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>Get all dashboards for the current user.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var dashboards = await _dashboardService.GetByUserAsync(userId.Value, cancellationToken);
        return Ok(dashboards);
    }

    /// <summary>Get a specific dashboard by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var dashboard = await _dashboardService.GetByIdAsync(id, userId.Value, cancellationToken);
        if (dashboard == null) return NotFound();
        return Ok(dashboard);
    }

    /// <summary>Create a new dashboard.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateDashboardRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var dashboard = await _dashboardService.CreateAsync(userId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = dashboard.Id }, dashboard);
    }

    /// <summary>Update a dashboard.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDashboardRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var dashboard = await _dashboardService.UpdateAsync(id, userId.Value, request, cancellationToken);
        if (dashboard == null) return NotFound();
        return Ok(dashboard);
    }

    /// <summary>Delete a dashboard.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var deleted = await _dashboardService.DeleteAsync(id, userId.Value, cancellationToken);
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
