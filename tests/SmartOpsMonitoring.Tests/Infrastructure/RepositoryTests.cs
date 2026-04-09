using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Infrastructure.Persistence;
using SmartOpsMonitoring.Infrastructure.Persistence.Repositories;

namespace SmartOpsMonitoring.Tests.Infrastructure;

/// <summary>
/// Integration-style unit tests for EF Core repository implementations using the
/// in-memory database provider. Each test class instance uses a uniquely named database
/// to guarantee isolation.
/// </summary>
public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initialises an <see cref="ApplicationDbContext"/> backed by a fresh in-memory database
    /// for each test.
    /// </summary>
    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }

    /// <summary>Disposes the database context after each test.</summary>
    public void Dispose() => _context.Dispose();

    // --- Generic Repository ---

    /// <summary>
    /// Verifies that <c>AddAsync</c> persists the entity so it can subsequently be
    /// retrieved via <c>GetByIdAsync</c>.
    /// </summary>
    [Fact]
    public async Task Repository_AddAsync_PersistsEntity()
    {
        var repo = new Repository<Host>(_context);
        var host = new Host { Name = "test-host", OsType = "Linux" };

        await repo.AddAsync(host);

        var fetched = await repo.GetByIdAsync(host.Id);
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be("test-host");
    }

    /// <summary>
    /// Verifies that <c>GetAllAsync</c> returns every entity that has been added to the store.
    /// </summary>
    [Fact]
    public async Task Repository_GetAllAsync_ReturnsAllEntities()
    {
        var repo = new Repository<Host>(_context);
        await repo.AddAsync(new Host { Name = "host-1", OsType = "Linux" });
        await repo.AddAsync(new Host { Name = "host-2", OsType = "Windows" });

        var all = await repo.GetAllAsync();

        all.Should().HaveCount(2);
    }

    /// <summary>
    /// Verifies that <c>GetByIdAsync</c> returns <c>null</c> when no entity with the
    /// specified identifier exists.
    /// </summary>
    [Fact]
    public async Task Repository_GetByIdAsync_UnknownId_ReturnsNull()
    {
        var repo = new Repository<Host>(_context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    /// <summary>
    /// Verifies that <c>UpdateAsync</c> persists property changes so they are visible
    /// on the next <c>GetByIdAsync</c> call.
    /// </summary>
    [Fact]
    public async Task Repository_UpdateAsync_ChangesAreSaved()
    {
        var repo = new Repository<Host>(_context);
        var host = new Host { Name = "original", OsType = "Linux" };
        await repo.AddAsync(host);

        host.Name = "updated";
        await repo.UpdateAsync(host);

        var fetched = await repo.GetByIdAsync(host.Id);
        fetched!.Name.Should().Be("updated");
    }

    /// <summary>
    /// Verifies that <c>DeleteAsync</c> removes the entity so a subsequent
    /// <c>GetByIdAsync</c> call returns <c>null</c>.
    /// </summary>
    [Fact]
    public async Task Repository_DeleteAsync_RemovesEntity()
    {
        var repo = new Repository<Host>(_context);
        var host = new Host { Name = "to-delete", OsType = "Linux" };
        await repo.AddAsync(host);

        await repo.DeleteAsync(host);

        var fetched = await repo.GetByIdAsync(host.Id);
        fetched.Should().BeNull();
    }

    /// <summary>
    /// Verifies that <c>FindAsync</c> returns only the entities that satisfy the given predicate.
    /// </summary>
    [Fact]
    public async Task Repository_FindAsync_FiltersByPredicate()
    {
        var repo = new Repository<Host>(_context);
        await repo.AddAsync(new Host { Name = "linux-host", OsType = "Linux" });
        await repo.AddAsync(new Host { Name = "windows-host", OsType = "Windows" });

        var result = await repo.FindAsync(h => h.OsType == "Linux");

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("linux-host");
    }

    /// <summary>
    /// Verifies that <c>ExistsAsync</c> returns <c>true</c> when at least one entity
    /// matches the supplied predicate.
    /// </summary>
    [Fact]
    public async Task Repository_ExistsAsync_ReturnsTrueWhenEntityExists()
    {
        var repo = new Repository<Host>(_context);
        var host = new Host { Name = "exists-host", OsType = "Linux" };
        await repo.AddAsync(host);

        var exists = await repo.ExistsAsync(h => h.Name == "exists-host");

        exists.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that <c>ExistsAsync</c> returns <c>false</c> when no entity matches
    /// the supplied predicate.
    /// </summary>
    [Fact]
    public async Task Repository_ExistsAsync_ReturnsFalseWhenEntityNotFound()
    {
        var repo = new Repository<Host>(_context);

        var exists = await repo.ExistsAsync(h => h.Name == "nonexistent");

        exists.Should().BeFalse();
    }

    // --- AlertRepository ---

    /// <summary>
    /// Verifies that <c>GetOpenAlertsAsync</c> returns only alerts with
    /// <see cref="AlertStatus.Open"/> status, excluding resolved or acknowledged ones.
    /// </summary>
    [Fact]
    public async Task AlertRepository_GetOpenAlertsAsync_ReturnsOnlyOpenAlerts()
    {
        var repo = new AlertRepository(_context);
        var hostId = Guid.NewGuid();
        await repo.AddAsync(new Alert { HostId = hostId, Title = "Open Alert", Message = "msg", Status = AlertStatus.Open, Severity = AlertSeverity.Warning });
        await repo.AddAsync(new Alert { HostId = hostId, Title = "Resolved Alert", Message = "msg", Status = AlertStatus.Resolved, Severity = AlertSeverity.Info });

        var result = await repo.GetOpenAlertsAsync();

        result.Should().HaveCount(1);
        result.First().Status.Should().Be(AlertStatus.Open);
    }

    /// <summary>
    /// Verifies that <c>GetByHostIdAsync</c> returns only the alerts associated with
    /// the specified host identifier.
    /// </summary>
    [Fact]
    public async Task AlertRepository_GetByHostIdAsync_ReturnsAlertsForHost()
    {
        var repo = new AlertRepository(_context);
        var hostId = Guid.NewGuid();
        await repo.AddAsync(new Alert { HostId = hostId, Title = "Alert 1", Message = "msg", Severity = AlertSeverity.Info });
        await repo.AddAsync(new Alert { HostId = Guid.NewGuid(), Title = "Alert 2", Message = "msg", Severity = AlertSeverity.Info });

        var result = await repo.GetByHostIdAsync(hostId);

        result.Should().HaveCount(1);
        result.First().HostId.Should().Be(hostId);
    }

    /// <summary>
    /// Verifies that <c>GetBySeverityAsync</c> returns only the alerts matching the
    /// requested <see cref="AlertSeverity"/>.
    /// </summary>
    [Fact]
    public async Task AlertRepository_GetBySeverityAsync_FiltersBySeverity()
    {
        var repo = new AlertRepository(_context);
        var hostId = Guid.NewGuid();
        await repo.AddAsync(new Alert { HostId = hostId, Title = "Critical Alert", Message = "msg", Severity = AlertSeverity.Critical });
        await repo.AddAsync(new Alert { HostId = hostId, Title = "Info Alert", Message = "msg", Severity = AlertSeverity.Info });

        var result = await repo.GetBySeverityAsync(AlertSeverity.Critical);

        result.Should().HaveCount(1);
        result.First().Severity.Should().Be(AlertSeverity.Critical);
    }

    // --- MetricRepository ---

    /// <summary>
    /// Verifies that <c>GetByHostIdAsync</c> returns only metrics recorded for the
    /// specified host identifier.
    /// </summary>
    [Fact]
    public async Task MetricRepository_GetByHostIdAsync_ReturnsMetricsForHost()
    {
        var repo = new MetricRepository(_context);
        var hostId = Guid.NewGuid();
        await repo.AddAsync(new Metric { HostId = hostId, MetricType = "cpu_usage", Value = 70, Unit = "percent" });
        await repo.AddAsync(new Metric { HostId = Guid.NewGuid(), MetricType = "cpu_usage", Value = 50, Unit = "percent" });

        var result = await repo.GetByHostIdAsync(hostId);

        result.Should().HaveCount(1);
        result.First().HostId.Should().Be(hostId);
    }

    /// <summary>
    /// Verifies that <c>GetByTypeAndRangeAsync</c> returns only the metrics that match
    /// the specified host, metric type, and fall within the given time range.
    /// </summary>
    [Fact]
    public async Task MetricRepository_GetByTypeAndRangeAsync_FiltersCorrectly()
    {
        var repo = new MetricRepository(_context);
        var hostId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        await repo.AddAsync(new Metric { HostId = hostId, MetricType = "cpu_usage", Value = 80, Unit = "percent", Timestamp = now });
        await repo.AddAsync(new Metric { HostId = hostId, MetricType = "cpu_usage", Value = 60, Unit = "percent", Timestamp = now.AddHours(-2) });
        await repo.AddAsync(new Metric { HostId = hostId, MetricType = "memory_bytes", Value = 1024, Unit = "bytes", Timestamp = now });

        var result = await repo.GetByTypeAndRangeAsync(hostId, "cpu_usage", now.AddMinutes(-30), now.AddMinutes(1));

        result.Should().HaveCount(1);
        result.First().Value.Should().Be(80);
    }

    // --- HostRepository ---

    /// <summary>
    /// Verifies that <c>GetByNameAsync</c> returns the host whose <c>Name</c> matches
    /// the supplied string.
    /// </summary>
    [Fact]
    public async Task HostRepository_GetByNameAsync_ReturnsMatchingHost()
    {
        var repo = new HostRepository(_context);
        await repo.AddAsync(new Host { Name = "unique-host", OsType = "Linux" });

        var result = await repo.GetByNameAsync("unique-host");

        result.Should().NotBeNull();
        result!.Name.Should().Be("unique-host");
    }

    /// <summary>
    /// Verifies that <c>GetByNameAsync</c> returns <c>null</c> when no host with the
    /// given name exists.
    /// </summary>
    [Fact]
    public async Task HostRepository_GetByNameAsync_NotFound_ReturnsNull()
    {
        var repo = new HostRepository(_context);

        var result = await repo.GetByNameAsync("nonexistent");

        result.Should().BeNull();
    }

    /// <summary>
    /// Verifies that <c>GetWithServiceNodesAsync</c> returns the host together with its
    /// eagerly loaded <see cref="ServiceNode"/> navigation collection.
    /// </summary>
    [Fact]
    public async Task HostRepository_GetWithServiceNodesAsync_IncludesServiceNodes()
    {
        var repo = new HostRepository(_context);
        var host = new Host { Name = "host-with-nodes", OsType = "Linux" };
        await repo.AddAsync(host);

        var node = new ServiceNode { Name = "api", Type = "web", HostId = host.Id, Port = 5000 };
        _context.ServiceNodes.Add(node);
        await _context.SaveChangesAsync();

        var result = await repo.GetWithServiceNodesAsync(host.Id);

        result.Should().NotBeNull();
        result!.ServiceNodes.Should().HaveCount(1);
        result.ServiceNodes.First().Name.Should().Be("api");
    }

    // --- ServiceNodeRepository ---

    /// <summary>
    /// Verifies that <c>GetByHostIdAsync</c> returns only service nodes belonging to
    /// the specified host identifier.
    /// </summary>
    [Fact]
    public async Task ServiceNodeRepository_GetByHostIdAsync_ReturnsNodesForHost()
    {
        var repo = new ServiceNodeRepository(_context);
        var hostId = Guid.NewGuid();
        await repo.AddAsync(new ServiceNode { Name = "node-1", Type = "web", HostId = hostId });
        await repo.AddAsync(new ServiceNode { Name = "node-2", Type = "db", HostId = Guid.NewGuid() });

        var result = await repo.GetByHostIdAsync(hostId);

        result.Should().HaveCount(1);
        result.First().HostId.Should().Be(hostId);
    }
}
