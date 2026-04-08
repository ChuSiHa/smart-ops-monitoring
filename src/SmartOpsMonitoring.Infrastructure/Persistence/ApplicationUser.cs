using Microsoft.AspNetCore.Identity;

namespace SmartOpsMonitoring.Infrastructure.Persistence;

/// <summary>
/// Application user extending ASP.NET Core Identity with additional profile fields.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>Gets or sets a human-readable display name for the user.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC timestamp when this user account was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
