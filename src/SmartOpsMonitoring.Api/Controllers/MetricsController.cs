using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.Services;

namespace SmartOpsMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MetricsController : ControllerBase
{
    private readonly IMetricService _metricService;

    public MetricsController(IMetricService metricService)
    {
        _metricService = metricService;
    }

    /// <summary>Ingest a new metric data point.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ingest([FromBody] IngestMetricRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var metric = await _metricService.IngestAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Query), null, metric);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Query metrics with optional filters and pagination.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Query([FromQuery] MetricQueryRequest query, CancellationToken cancellationToken)
    {
        var result = await _metricService.QueryAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>Get the latest value for each metric type from a specific device.</summary>
    [HttpGet("devices/{deviceId:int}/latest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatest(int deviceId, CancellationToken cancellationToken)
    {
        try
        {
            var metrics = await _metricService.GetLatestByDeviceAsync(deviceId, cancellationToken);
            return Ok(metrics);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
