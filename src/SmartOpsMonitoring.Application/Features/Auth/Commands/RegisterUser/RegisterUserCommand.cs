namespace SmartOpsMonitoring.Application.Features.Auth.Commands.RegisterUser;

/// <summary>
/// Command to register a new user account.
/// </summary>
public class RegisterUserCommand : ICommand<RegisterResultDto>
{
    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's plaintext password.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets an optional display name; defaults to email if absent.</summary>
    public string? DisplayName { get; set; }
}
