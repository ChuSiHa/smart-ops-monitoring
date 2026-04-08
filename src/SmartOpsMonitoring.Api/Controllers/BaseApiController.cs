using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SmartOpsMonitoring.Api.Controllers;

/// <summary>
/// Abstract base controller that wires up MediatR <see cref="ISender"/> for all derived controllers.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>Gets the MediatR sender used to dispatch commands and queries.</summary>
    protected readonly ISender Sender;

    /// <summary>
    /// Initialises a new instance of <see cref="BaseApiController"/>.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    protected BaseApiController(ISender sender)
    {
        Sender = sender;
    }
}
