using SmartOpsMonitoring.Application.Contracts;

namespace SmartOpsMonitoring.Application.Features.Auth.Commands.RegisterUser;

/// <summary>
/// Handles <see cref="RegisterUserCommand"/> by delegating to <see cref="IAuthService"/>.
/// </summary>
public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterResultDto>
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initialises a new instance of <see cref="RegisterUserCommandHandler"/>.
    /// </summary>
    /// <param name="authService">The authentication service.</param>
    public RegisterUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Executes the command by calling the authentication service to register the user.
    /// </summary>
    /// <param name="request">The registration command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="RegisterResultDto"/> confirming registration.</returns>
    public async Task<RegisterResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        => await _authService.RegisterAsync(request.Email, request.Password, request.DisplayName, cancellationToken);
}
