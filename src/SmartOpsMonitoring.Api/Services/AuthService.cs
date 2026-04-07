using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SmartOpsMonitoring.Api.Data;
using SmartOpsMonitoring.Api.Data.Repositories;
using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;
using SmartOpsMonitoring.Api.Models;

namespace SmartOpsMonitoring.Api.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IRepository<User> userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.FindAsync(u => u.Username == request.Username && u.IsActive, cancellationToken);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(GetTokenExpiryHours());

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = MapToUserResponse(user)
        };
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var userExists = await _userRepository.ExistsAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken);
        if (userExists)
            throw new InvalidOperationException("Username or email already exists.");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _userRepository.AddAsync(user, cancellationToken);
        return MapToUserResponse(created);
    }

    public async Task<UserResponse?> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user == null ? null : MapToUserResponse(user);
    }

    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured.");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(GetTokenExpiryHours()),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetTokenExpiryHours()
        => int.TryParse(_configuration["Jwt:ExpiryHours"], out var hours) ? hours : 8;

    private static UserResponse MapToUserResponse(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        Role = user.Role,
        CreatedAt = user.CreatedAt,
        LastLoginAt = user.LastLoginAt,
        IsActive = user.IsActive
    };
}
