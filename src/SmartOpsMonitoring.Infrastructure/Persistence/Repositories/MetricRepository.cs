using Microsoft.EntityFrameworkCore;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IMetricRepository"/>.
/// </summary>
public class MetricRepository : Repository<Metric>, IMetricRepository
{
    /// <summary>
    /// Initialises a new instance of <see cref="MetricRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public MetricRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Metric>> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default)
        => await _context.Metrics
            .Where(m => m.HostId == hostId)
            .OrderByDescending(m => m.Timestamp)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<Metric>> GetByTypeAndRangeAsync(
        Guid hostId, string metricType, DateTime from, DateTime to, CancellationToken cancellationToken = default)
        => await _context.Metrics
            .Where(m => m.HostId == hostId
                     && m.MetricType == metricType
                     && m.Timestamp >= from
                     && m.Timestamp <= to)
            .OrderBy(m => m.Timestamp)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<Metric>> GetLatestByHostAsync(Guid hostId, CancellationToken cancellationToken = default)
        => await _context.Metrics
            .Where(m => m.HostId == hostId)
            .GroupBy(m => m.MetricType)
            .Select(g => g.OrderByDescending(m => m.Timestamp).First())
            .ToListAsync(cancellationToken);
}
