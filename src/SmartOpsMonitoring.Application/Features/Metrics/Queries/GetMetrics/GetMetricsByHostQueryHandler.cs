namespace SmartOpsMonitoring.Application.Features.Metrics.Queries.GetMetrics;

/// <summary>
/// Handles <see cref="GetMetricsByHostQuery"/> by querying the metric repository.
/// </summary>
public class GetMetricsByHostQueryHandler : IQueryHandler<GetMetricsByHostQuery, IEnumerable<MetricDto>>
{
    private readonly IMetricRepository _metricRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="GetMetricsByHostQueryHandler"/>.
    /// </summary>
    /// <param name="metricRepository">The metric repository.</param>
    public GetMetricsByHostQueryHandler(IMetricRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }

    /// <summary>
    /// Executes the query and returns matching metrics mapped to DTOs.
    /// </summary>
    /// <param name="request">The query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of <see cref="MetricDto"/> matching the query.</returns>
    public async Task<IEnumerable<MetricDto>> Handle(GetMetricsByHostQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Metric> metrics;

        if (!string.IsNullOrWhiteSpace(request.MetricType) && request.From.HasValue && request.To.HasValue)
        {
            metrics = await _metricRepository.GetByTypeAndRangeAsync(
                request.HostId, request.MetricType, request.From.Value, request.To.Value, cancellationToken);
        }
        else
        {
            metrics = await _metricRepository.GetByHostIdAsync(request.HostId, cancellationToken);
        }

        return metrics.Select(m => m.Adapt<MetricDto>());
    }
}
