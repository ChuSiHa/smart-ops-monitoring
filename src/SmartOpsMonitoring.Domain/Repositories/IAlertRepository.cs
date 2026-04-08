using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Enums;

namespace SmartOpsMonitoring.Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="Alert"/> entities with alert-specific queries.
/// </summary>
public interface IAlertRepository : IRepository<Alert>
{
    /// <summary>Retrieves all alerts with <see cref="AlertStatus.Open"/> status.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Open alerts.</returns>
    Task<IEnumerable<Alert>> GetOpenAlertsAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves all alerts associated with a specific host.</summary>
    /// <param name="hostId">The host identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Alerts for the specified host.</returns>
    Task<IEnumerable<Alert>> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all alerts with a specific severity level.</summary>
    /// <param name="severity">The severity to filter on.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Alerts matching the specified severity.</returns>
    Task<IEnumerable<Alert>> GetBySeverityAsync(AlertSeverity severity, CancellationToken cancellationToken = default);
}
