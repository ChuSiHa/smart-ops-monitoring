namespace SmartOpsMonitoring.Application.DTOs.Auth;

/// <summary>
/// Result DTO returned after a successful login, containing the JWT bearer token.
/// </summary>
public class LoginResultDto
{
    /// <summary>Gets or sets the JWT bearer token.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC expiry time of the token.</summary>
    public DateTime ExpiresAt { get; set; }
}
