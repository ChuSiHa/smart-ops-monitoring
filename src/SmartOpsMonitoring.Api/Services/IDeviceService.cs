using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;

namespace SmartOpsMonitoring.Api.Services;

public interface IDeviceService
{
    Task<IEnumerable<DeviceResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DeviceResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DeviceResponse> CreateAsync(CreateDeviceRequest request, CancellationToken cancellationToken = default);
    Task<DeviceResponse?> UpdateAsync(int id, UpdateDeviceRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
