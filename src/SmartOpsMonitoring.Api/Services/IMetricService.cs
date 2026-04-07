using SmartOpsMonitoring.Api.DTOs.Requests;
using SmartOpsMonitoring.Api.DTOs.Responses;

namespace SmartOpsMonitoring.Api.Services;

public interface IMetricService
{
    Task<MetricResponse> IngestAsync(IngestMetricRequest request, CancellationToken cancellationToken = default);
    Task<PagedMetricResponse> QueryAsync(MetricQueryRequest query, CancellationToken cancellationToken = default);
    Task<IEnumerable<MetricResponse>> GetLatestByDeviceAsync(int deviceId, CancellationToken cancellationToken = default);
}
