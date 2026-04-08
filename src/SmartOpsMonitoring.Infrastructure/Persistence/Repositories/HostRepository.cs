using Microsoft.EntityFrameworkCore;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IHostRepository"/>.
/// </summary>
public class HostRepository : Repository<Host>, IHostRepository
{
    /// <summary>
    /// Initialises a new instance of <see cref="HostRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public HostRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<Host?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _context.Hosts.FirstOrDefaultAsync(h => h.Name == name, cancellationToken);

    /// <inheritdoc/>
    public async Task<Host?> GetWithServiceNodesAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Hosts
            .Include(h => h.ServiceNodes)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
}
