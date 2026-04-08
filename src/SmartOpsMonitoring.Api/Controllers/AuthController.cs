namespace SmartOpsMonitoring.Api.Controllers;

/// <summary>
/// Handles user registration and authentication, returning JWT tokens on success.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initialises a new instance of <see cref="AuthController"/>.
    /// </summary>
    /// <param name="sender">The MediatR sender used to dispatch auth commands.</param>
    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="command">The registration command containing email, password, and optional display name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="RegisterResultDto"/> on success, or validation errors on failure.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        try
        {
            var result = await _sender.Send(command, ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticates a user and returns a JWT bearer token on success.
    /// </summary>
    /// <param name="command">The login command containing email and password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="LoginResultDto"/> containing the JWT token on success, or 401 on invalid credentials.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        try
        {
            var result = await _sender.Send(command, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}

