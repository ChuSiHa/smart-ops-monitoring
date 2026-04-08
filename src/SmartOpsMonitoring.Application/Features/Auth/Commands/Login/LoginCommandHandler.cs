using SmartOpsMonitoring.Application.Contracts;

namespace SmartOpsMonitoring.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handles <see cref="LoginCommand"/> by delegating to <see cref="IAuthService"/>.
/// </summary>
public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResultDto>
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initialises a new instance of <see cref="LoginCommandHandler"/>.
    /// </summary>
    /// <param name="authService">The authentication service.</param>
    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Executes the command by calling the authentication service to authenticate the user.
    /// </summary>
    /// <param name="request">The login command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="LoginResultDto"/> containing the JWT token.</returns>
    public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        => await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
}
