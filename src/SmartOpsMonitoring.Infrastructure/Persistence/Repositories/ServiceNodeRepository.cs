using Microsoft.EntityFrameworkCore;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IServiceNodeRepository"/>.
/// </summary>
public class ServiceNodeRepository : Repository<ServiceNode>, IServiceNodeRepository
{
    /// <summary>
    /// Initialises a new instance of <see cref="ServiceNodeRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ServiceNodeRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServiceNode>> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default)
        => await _context.ServiceNodes
            .Where(s => s.HostId == hostId)
            .ToListAsync(cancellationToken);
}
