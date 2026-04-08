using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOpsMonitoring.Application.Features.ServiceNodes.Commands.CreateServiceNode;
using SmartOpsMonitoring.Application.Features.ServiceNodes.Queries.GetServiceNodesByHost;

namespace SmartOpsMonitoring.Api.Controllers;

/// <summary>
/// Exposes endpoints for managing service nodes on monitored hosts.
/// </summary>
[Authorize]
public class ServiceNodesController : BaseApiController
{
    /// <summary>
    /// Initialises a new instance of <see cref="ServiceNodesController"/>.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    public ServiceNodesController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Returns all service nodes belonging to the specified host.
    /// </summary>
    /// <param name="hostId">The host identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet]
    public async Task<IActionResult> GetByHost([FromQuery] Guid hostId, CancellationToken ct)
        => Ok(await Sender.Send(new GetServiceNodesByHostQuery { HostId = hostId }, ct));

    /// <summary>
    /// Creates a new service node on a host.
    /// </summary>
    /// <param name="command">The create service node command.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceNodeCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);
        return CreatedAtAction(nameof(GetByHost), new { hostId = result.HostId }, result);
    }
}
