using SmartOpsMonitoring.Application.DTOs.Auth;

namespace SmartOpsMonitoring.Application.Contracts;

/// <summary>
/// Defines authentication operations used by the application layer.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the provided credentials.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's plaintext password.</param>
    /// <param name="displayName">Optional display name; defaults to email if not provided.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="RegisterResultDto"/> indicating success or failure.</returns>
    Task<RegisterResultDto> RegisterAsync(string email, string password, string? displayName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user and returns a JWT token on success.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's plaintext password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="LoginResultDto"/> containing the JWT token.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid.</exception>
    Task<LoginResultDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
