namespace SmartOpsMonitoring.Api.Controllers;

/// <summary>
/// Exposes endpoints for ingesting and querying metric data.
/// </summary>
[Authorize]
public class MetricsController : BaseApiController
{
    /// <summary>
    /// Initialises a new instance of <see cref="MetricsController"/>.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    public MetricsController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Ingests a new metric data point.
    /// </summary>
    /// <param name="command">The ingest metric command.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost("ingest")]
    public async Task<IActionResult> Ingest([FromBody] IngestMetricCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Returns metrics for a specific host, optionally filtered by type and time range.
    /// </summary>
    /// <param name="hostId">The host identifier.</param>
    /// <param name="request">Query parameters for filtering by metric type and time range.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet("host/{hostId:guid}")]
    public async Task<IActionResult> GetByHost(
        Guid hostId,
        [FromQuery] GetMetricsByHostRequest request,
        CancellationToken ct)
        => Ok(await Sender.Send(new GetMetricsByHostQuery
        {
            HostId = hostId,
            MetricType = request.MetricType,
            From = request.From,
            To = request.To
        }, ct));
}
