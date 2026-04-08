namespace SmartOpsMonitoring.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user and obtain a JWT bearer token.
/// </summary>
public class LoginCommand : ICommand<LoginResultDto>
{
    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's plaintext password.</summary>
    public string Password { get; set; } = string.Empty;
}
