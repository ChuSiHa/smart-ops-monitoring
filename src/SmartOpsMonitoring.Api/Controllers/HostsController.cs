using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOpsMonitoring.Application.Features.Hosts.Commands.CreateHost;
using SmartOpsMonitoring.Application.Features.Hosts.Queries.GetHostById;
using SmartOpsMonitoring.Application.Features.Hosts.Queries.GetHosts;

namespace SmartOpsMonitoring.Api.Controllers;

/// <summary>
/// Exposes CRUD endpoints for managing monitored hosts.
/// </summary>
[Authorize]
public class HostsController : BaseApiController
{
    /// <summary>
    /// Initialises a new instance of <see cref="HostsController"/>.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    public HostsController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Returns all registered hosts.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await Sender.Send(new GetHostsQuery(), ct));

    /// <summary>
    /// Returns a single host by its identifier.
    /// </summary>
    /// <param name="id">The host identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new GetHostByIdQuery { Id = id }, ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new host registration.
    /// </summary>
    /// <param name="command">The create host command.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHostCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
