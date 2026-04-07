using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.Services;

namespace SmartOpsMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    /// <summary>Get all devices.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var devices = await _deviceService.GetAllAsync(cancellationToken);
        return Ok(devices);
    }

    /// <summary>Get a device by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var device = await _deviceService.GetByIdAsync(id, cancellationToken);
        if (device == null) return NotFound();
        return Ok(device);
    }

    /// <summary>Create a new device.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateDeviceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var device = await _deviceService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Update an existing device.</summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDeviceRequest request, CancellationToken cancellationToken)
    {
        var device = await _deviceService.UpdateAsync(id, request, cancellationToken);
        if (device == null) return NotFound();
        return Ok(device);
    }

    /// <summary>Delete a device.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _deviceService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
