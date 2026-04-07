using Microsoft.EntityFrameworkCore;
using SmartOpsMonitoring.Api.Models;

namespace SmartOpsMonitoring.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Metric> Metrics => Set<Metric>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<Dashboard> Dashboards => Set<Dashboard>();
    public DbSet<DataSource> DataSources => Set<DataSource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Role).HasDefaultValue("Viewer");
            entity.Property(u => u.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasIndex(d => d.Name).IsUnique();
            entity.Property(d => d.Status).HasDefaultValue("Offline");
            entity.Property(d => d.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Metric>(entity =>
        {
            entity.HasOne(m => m.Device)
                  .WithMany(d => d.Metrics)
                  .HasForeignKey(m => m.DeviceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(m => new { m.DeviceId, m.MetricType, m.Timestamp });
        });

        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasOne(a => a.Device)
                  .WithMany(d => d.Alerts)
                  .HasForeignKey(a => a.DeviceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => new { a.DeviceId, a.Status });
            entity.Property(a => a.Severity).HasDefaultValue("Info");
            entity.Property(a => a.Status).HasDefaultValue("Open");
        });

        modelBuilder.Entity<Dashboard>(entity =>
        {
            entity.HasOne(d => d.User)
                  .WithMany(u => u.Dashboards)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DataSource>(entity =>
        {
            entity.HasIndex(ds => ds.Name).IsUnique();
            entity.Property(ds => ds.IsActive).HasDefaultValue(true);
        });
    }
}
