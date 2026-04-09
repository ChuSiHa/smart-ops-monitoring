using FluentAssertions;
using Moq;
using SmartOpsMonitoring.Application.Features.Alerts.Queries.GetAlerts;
using SmartOpsMonitoring.Application.Mappings;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Tests.Application.Handlers;

public class GetAlertsQueryHandlerTests
{
    private readonly Mock<IAlertRepository> _alertRepositoryMock = new();
    private readonly GetAlertsQueryHandler _handler;

    public GetAlertsQueryHandlerTests()
    {
        MappingConfig.RegisterMappings();
        _handler = new GetAlertsQueryHandler(_alertRepositoryMock.Object);
    }

    private static Alert MakeAlert(AlertSeverity severity = AlertSeverity.Warning, AlertStatus status = AlertStatus.Open, Guid? hostId = null) => new()
    {
        Id = Guid.NewGuid(),
        HostId = hostId ?? Guid.NewGuid(),
        Title = "Test",
        Message = "Test message",
        Severity = severity,
        Status = status
    };

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllAlerts()
    {
        var alerts = new List<Alert> { MakeAlert(), MakeAlert() };
        _alertRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        var result = await _handler.Handle(new GetAlertsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

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

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmpty()
    {
        _alertRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Alert>());

        var result = await _handler.Handle(new GetAlertsQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

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
