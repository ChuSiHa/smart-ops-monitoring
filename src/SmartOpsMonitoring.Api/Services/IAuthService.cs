using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;

namespace SmartOpsMonitoring.Api.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse?> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default);
}
