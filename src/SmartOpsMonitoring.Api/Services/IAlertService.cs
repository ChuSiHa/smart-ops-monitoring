using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;

namespace SmartOpsMonitoring.Api.Services;

public interface IAlertService
{
    Task<IEnumerable<AlertResponse>> GetAllAsync(string? status = null, string? severity = null, CancellationToken cancellationToken = default);
    Task<AlertResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AlertResponse> CreateAsync(CreateAlertRequest request, CancellationToken cancellationToken = default);
    Task<AlertResponse?> UpdateAsync(int id, UpdateAlertRequest request, int currentUserId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
