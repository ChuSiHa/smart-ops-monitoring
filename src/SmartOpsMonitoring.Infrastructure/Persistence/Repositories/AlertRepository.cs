namespace SmartOpsMonitoring.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAlertRepository"/>.
/// </summary>
public class AlertRepository : Repository<Alert>, IAlertRepository
{
    /// <summary>
    /// Initialises a new instance of <see cref="AlertRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AlertRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Alert>> GetOpenAlertsAsync(CancellationToken cancellationToken = default)
        => await _context.Alerts
            .Where(a => a.Status == AlertStatus.Open)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<Alert>> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default)
        => await _context.Alerts
            .Where(a => a.HostId == hostId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<Alert>> GetBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default)
        => await _context.Alerts
            .Where(a => a.Severity == severity)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
}
