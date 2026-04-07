using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;

namespace SmartOpsMonitoring.Api.Services;

public interface IDataSourceService
{
    Task<IEnumerable<DataSourceResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DataSourceResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DataSourceResponse> CreateAsync(CreateDataSourceRequest request, CancellationToken cancellationToken = default);
    Task<DataSourceResponse?> UpdateAsync(int id, UpdateDataSourceRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<DataSourceResponse?> TestConnectionAsync(int id, CancellationToken cancellationToken = default);
}
