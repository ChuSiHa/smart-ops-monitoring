using SmartOpsMonitoring.Domain.Entities;

namespace SmartOpsMonitoring.Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="Metric"/> entities with time-range and host queries.
/// </summary>
public interface IMetricRepository : IRepository<Metric>
{
    /// <summary>Retrieves all metrics for a given host.</summary>
    /// <param name="hostId">The host identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All metrics recorded for the host.</returns>
    Task<IEnumerable<Metric>> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves metrics for a host filtered by type and time range.</summary>
    /// <param name="hostId">The host identifier.</param>
    /// <param name="metricType">The metric type to filter on.</param>
    /// <param name="from">Start of the time range (inclusive).</param>
    /// <param name="to">End of the time range (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Metrics matching the filter criteria.</returns>
    Task<IEnumerable<Metric>> GetByTypeAndRangeAsync(Guid hostId, string metricType, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the most recently recorded metrics for a host (one per metric type).</summary>
    /// <param name="hostId">The host identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The latest metric per type for the specified host.</returns>
    Task<IEnumerable<Metric>> GetLatestByHostAsync(Guid hostId, CancellationToken cancellationToken = default);
}
