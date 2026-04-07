using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.Services;

namespace SmartOpsMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DataSourcesController : ControllerBase
{
    private readonly IDataSourceService _dataSourceService;

    public DataSourcesController(IDataSourceService dataSourceService)
    {
        _dataSourceService = dataSourceService;
    }

    /// <summary>Get all data sources.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var sources = await _dataSourceService.GetAllAsync(cancellationToken);
        return Ok(sources);
    }

    /// <summary>Get a data source by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var source = await _dataSourceService.GetByIdAsync(id, cancellationToken);
        if (source == null) return NotFound();
        return Ok(source);
    }

    /// <summary>Create a new data source.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateDataSourceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var source = await _dataSourceService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = source.Id }, source);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Update a data source.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDataSourceRequest request, CancellationToken cancellationToken)
    {
        var source = await _dataSourceService.UpdateAsync(id, request, cancellationToken);
        if (source == null) return NotFound();
        return Ok(source);
    }

    /// <summary>Delete a data source.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _dataSourceService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>Test the connection for a data source.</summary>
    [HttpPost("{id:int}/test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TestConnection(int id, CancellationToken cancellationToken)
    {
        var source = await _dataSourceService.TestConnectionAsync(id, cancellationToken);
        if (source == null) return NotFound();
        return Ok(source);
    }
}
