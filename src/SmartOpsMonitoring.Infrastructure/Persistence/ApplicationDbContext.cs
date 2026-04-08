using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartOpsMonitoring.Domain.Entities;

namespace SmartOpsMonitoring.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for SmartOps Monitoring.
/// Extends <see cref="IdentityDbContext{TUser}"/> to include ASP.NET Core Identity tables.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>Gets or sets the hosts table.</summary>
    public DbSet<Host> Hosts => Set<Host>();

    /// <summary>Gets or sets the service nodes table.</summary>
    public DbSet<ServiceNode> ServiceNodes => Set<ServiceNode>();

    /// <summary>Gets or sets the metrics table.</summary>
    public DbSet<Metric> Metrics => Set<Metric>();

    /// <summary>Gets or sets the alerts table.</summary>
    public DbSet<Alert> Alerts => Set<Alert>();

    /// <summary>
    /// Initialises a new instance of <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="options">EF Core context options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures entity mappings, indexes, and relationships.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Host>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
            entity.Property(h => h.IpAddress).HasMaxLength(45);
            entity.Property(h => h.OsType).HasMaxLength(100);
            entity.Property(h => h.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            entity.HasMany(h => h.ServiceNodes)
                  .WithOne(s => s.Host)
                  .HasForeignKey(s => s.HostId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(h => h.Name).IsUnique();
        });

        modelBuilder.Entity<ServiceNode>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
            entity.Property(s => s.Type).IsRequired().HasMaxLength(100);
            entity.HasIndex(s => s.HostId);
        });

        modelBuilder.Entity<Metric>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.MetricType).IsRequired().HasMaxLength(100);
            entity.Property(m => m.Unit).HasMaxLength(50);
            entity.Property(m => m.Labels)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null)
                         ?? new Dictionary<string, string>());
            // Composite index for host-based time-series queries
            entity.HasIndex(m => new { m.HostId, m.Timestamp });
        });

        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Title).IsRequired().HasMaxLength(200);
            entity.Property(a => a.Message).HasMaxLength(2000);
            // Index to speed up open/severity queries
            entity.HasIndex(a => new { a.Status, a.Severity });
            entity.HasIndex(a => a.HostId);
        });
    }
}
