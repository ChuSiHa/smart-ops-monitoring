namespace SmartOpsMonitoring.Application.DTOs.Auth;

/// <summary>
/// Result DTO returned after a successful user registration.
/// </summary>
public class RegisterResultDto
{
    /// <summary>Gets or sets a confirmation message.</summary>
    public string Message { get; set; } = string.Empty;
}
