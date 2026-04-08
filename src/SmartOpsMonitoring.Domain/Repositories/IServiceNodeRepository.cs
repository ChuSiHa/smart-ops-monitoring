using SmartOpsMonitoring.Domain.Entities;

namespace SmartOpsMonitoring.Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="ServiceNode"/> entities.
/// </summary>
public interface IServiceNodeRepository : IRepository<ServiceNode>
{
    /// <summary>Retrieves all service nodes associated with a specific host.</summary>
    /// <param name="hostId">The host identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Service nodes belonging to the specified host.</returns>
    Task<IEnumerable<ServiceNode>> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default);
}
