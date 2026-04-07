using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;

namespace SmartOpsMonitoring.Api.Services;

public interface IDashboardService
{
    Task<IEnumerable<DashboardResponse>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<DashboardResponse?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<DashboardResponse> CreateAsync(int userId, CreateDashboardRequest request, CancellationToken cancellationToken = default);
    Task<DashboardResponse?> UpdateAsync(int id, int userId, UpdateDashboardRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
}
