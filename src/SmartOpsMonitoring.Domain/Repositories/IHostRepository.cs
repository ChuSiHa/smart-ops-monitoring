using SmartOpsMonitoring.Domain.Entities;

namespace SmartOpsMonitoring.Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="Host"/> entities with host-specific queries.
/// </summary>
public interface IHostRepository : IRepository<Host>
{
    /// <summary>Finds a host by its unique name.</summary>
    /// <param name="name">The host name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The matching host, or <c>null</c> if not found.</returns>
    Task<Host?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a host along with its associated service nodes.</summary>
    /// <param name="id">The host identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The host including service nodes, or <c>null</c> if not found.</returns>
    Task<Host?> GetWithServiceNodesAsync(Guid id, CancellationToken cancellationToken = default);
}
