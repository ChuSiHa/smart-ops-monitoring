using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartOpsMonitoring.Application.Contracts;
using SmartOpsMonitoring.Application.DTOs.Auth;
using SmartOpsMonitoring.Infrastructure.Persistence;

namespace SmartOpsMonitoring.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IAuthService"/> using ASP.NET Core Identity and JWT token generation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initialises a new instance of <see cref="AuthService"/>.
    /// </summary>
    /// <param name="userManager">The ASP.NET Core Identity user manager.</param>
    /// <param name="signInManager">The ASP.NET Core Identity sign-in manager.</param>
    /// <param name="configuration">The application configuration.</param>
    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Registers a new user account using ASP.NET Core Identity.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's plaintext password.</param>
    /// <param name="displayName">Optional display name; defaults to email if not provided.</param>
    /// <param name="cancellationToken">Cancellation token (not used by Identity but accepted for interface compliance).</param>
    /// <returns>A <see cref="RegisterResultDto"/> on success.</returns>
    /// <exception cref="InvalidOperationException">Thrown when Identity reports registration errors.</exception>
    public async Task<RegisterResultDto> RegisterAsync(
        string email,
        string password,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName ?? email
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Registration failed: {errors}");
        }

        return new RegisterResultDto { Message = "User registered successfully." };
    }

    /// <summary>
    /// Authenticates a user and generates a signed JWT bearer token on success.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's plaintext password.</param>
    /// <param name="cancellationToken">Cancellation token (not used by Identity but accepted for interface compliance).</param>
    /// <returns>A <see cref="LoginResultDto"/> containing the JWT token and its expiry.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the email is not found or the password is incorrect.</exception>
    public async Task<LoginResultDto> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Invalid credentials.");

        return BuildJwtToken(user);
    }

    /// <summary>
    /// Builds a signed JWT bearer token for the specified user using configuration settings.
    /// </summary>
    /// <param name="user">The authenticated user for whom the token is generated.</param>
    /// <returns>A <see cref="LoginResultDto"/> containing the signed token and its UTC expiry time.</returns>
    private LoginResultDto BuildJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpiryHours"] ?? "24"));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.Email!)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

        return new LoginResultDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiry
        };
    }
}
