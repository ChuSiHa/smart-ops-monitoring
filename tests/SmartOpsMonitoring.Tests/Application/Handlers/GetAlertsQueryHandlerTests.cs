using FluentAssertions;
using Moq;
using SmartOpsMonitoring.Application.Features.Alerts.Queries.GetAlerts;
using SmartOpsMonitoring.Application.Mappings;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Tests.Application.Handlers;

/// <summary>
/// Unit tests for <see cref="GetAlertsQueryHandler"/>.
/// </summary>
public class GetAlertsQueryHandlerTests
{
    private readonly Mock<IAlertRepository> _alertRepositoryMock = new();
    private readonly GetAlertsQueryHandler _handler;

    /// <summary>Initialises mocks and registers Mapster mappings required by the handler.</summary>
    public GetAlertsQueryHandlerTests()
    {
        MappingConfig.RegisterMappings();
        _handler = new GetAlertsQueryHandler(_alertRepositoryMock.Object);
    }

    /// <summary>Creates a test <see cref="Alert"/> with the specified severity, status, and host.</summary>
    private static Alert MakeAlert(AlertSeverity severity = AlertSeverity.Warning, AlertStatus status = AlertStatus.Open, Guid? hostId = null) => new()
    {
        Id = Guid.NewGuid(),
        HostId = hostId ?? Guid.NewGuid(),
        Title = "Test",
        Message = "Test message",
        Severity = severity,
        Status = status
    };

    /// <summary>
    /// Verifies that when no filters are specified the handler calls <c>GetAllAsync</c>
    /// and returns all available alerts.
    /// </summary>
    [Fact]
    public async Task Handle_NoFilters_ReturnsAllAlerts()
    {
        var alerts = new List<Alert> { MakeAlert(), MakeAlert() };
        _alertRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        var result = await _handler.Handle(new GetAlertsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    /// <summary>
    /// Verifies that providing a <c>HostId</c> filter routes the query to
    /// <c>GetByHostIdAsync</c> rather than the general <c>GetAllAsync</c>.
    /// </summary>
    [Fact]
    public async Task Handle_HostIdFilter_UsesGetByHostId()
    {
        var hostId = Guid.NewGuid();
        var alerts = new List<Alert> { MakeAlert(hostId: hostId) };
        _alertRepositoryMock.Setup(r => r.GetByHostIdAsync(hostId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        var result = await _handler.Handle(new GetAlertsQuery { HostId = hostId }, CancellationToken.None);

        result.Should().HaveCount(1);
        _alertRepositoryMock.Verify(r => r.GetByHostIdAsync(hostId, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that providing only a <c>Severity</c> filter routes the query to
    /// <c>GetBySeverityAsync</c> for efficient database-side filtering.
    /// </summary>
    [Fact]
    public async Task Handle_SeverityOnlyFilter_UsesGetBySeverity()
    {
        var alerts = new List<Alert> { MakeAlert(AlertSeverity.Critical) };
        _alertRepositoryMock.Setup(r => r.GetBySeverityAsync(AlertSeverity.Critical, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        var result = await _handler.Handle(new GetAlertsQuery { Severity = "Critical" }, CancellationToken.None);

        result.Should().HaveCount(1);
        _alertRepositoryMock.Verify(r => r.GetBySeverityAsync(AlertSeverity.Critical, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that providing only an "Open" <c>Status</c> filter routes the query to
    /// <c>GetOpenAlertsAsync</c> for efficient database-side filtering.
    /// </summary>
    [Fact]
    public async Task Handle_OpenStatusOnlyFilter_UsesGetOpenAlerts()
    {
        var alerts = new List<Alert> { MakeAlert(status: AlertStatus.Open) };
        _alertRepositoryMock.Setup(r => r.GetOpenAlertsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        var result = await _handler.Handle(new GetAlertsQuery { Status = "Open" }, CancellationToken.None);

        result.Should().HaveCount(1);
        _alertRepositoryMock.Verify(r => r.GetOpenAlertsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that combining both <c>Status</c> and <c>Severity</c> filters falls back to
    /// <c>GetAllAsync</c> followed by in-memory filtering, returning only the intersection.
    /// </summary>
    [Fact]
    public async Task Handle_StatusAndSeverityFilter_FiltersInMemory()
    {
        var alerts = new List<Alert>
        {
            MakeAlert(AlertSeverity.Critical, AlertStatus.Open),
            MakeAlert(AlertSeverity.Warning, AlertStatus.Open),
            MakeAlert(AlertSeverity.Critical, AlertStatus.Resolved)
        };
        _alertRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        var result = await _handler.Handle(
            new GetAlertsQuery { Status = "Open", Severity = "Critical" },
            CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Status.Should().Be(AlertStatus.Open);
        result.First().Severity.Should().Be(AlertSeverity.Critical);
    }

    /// <summary>
    /// Verifies that when the repository contains no alerts, the handler returns an empty collection.
    /// </summary>
    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmpty()
    {
        _alertRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Alert>());

        var result = await _handler.Handle(new GetAlertsQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that an unrecognised <c>Severity</c> string falls back to <c>GetAllAsync</c>
    /// without applying any severity filtering.
    /// </summary>
    [Fact]
    public async Task Handle_InvalidSeverityFilter_FallsBackToGetAll()
    {
        var alerts = new List<Alert> { MakeAlert() };
        _alertRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        var result = await _handler.Handle(new GetAlertsQuery { Severity = "NotASeverity" }, CancellationToken.None);

        _alertRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
