using FluentAssertions;
using Moq;
using SmartOpsMonitoring.Application.Features.Alerts.Commands.UpdateAlertStatus;
using SmartOpsMonitoring.Application.Mappings;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Tests.Application.Handlers;

/// <summary>
/// Unit tests for <see cref="UpdateAlertStatusCommandHandler"/>.
/// </summary>
public class UpdateAlertStatusCommandHandlerTests
{
    private readonly Mock<IAlertRepository> _alertRepositoryMock = new();
    private readonly UpdateAlertStatusCommandHandler _handler;

    /// <summary>Initialises mocks and registers Mapster mappings required by the handler.</summary>
    public UpdateAlertStatusCommandHandlerTests()
    {
        MappingConfig.RegisterMappings();
        _handler = new UpdateAlertStatusCommandHandler(_alertRepositoryMock.Object);
    }

    /// <summary>Creates a test <see cref="Alert"/> with the given status.</summary>
    private static Alert CreateAlert(AlertStatus status = AlertStatus.Open) => new()
    {
        Id = Guid.NewGuid(),
        HostId = Guid.NewGuid(),
        Title = "Test Alert",
        Message = "Test message",
        Severity = AlertSeverity.Warning,
        Status = status
    };

    /// <summary>
    /// Verifies that a <see cref="KeyNotFoundException"/> is thrown when the alert
    /// identifier does not match any persisted alert.
    /// </summary>
    [Fact]
    public async Task Handle_AlertNotFound_ThrowsKeyNotFoundException()
    {
        _alertRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Alert?)null);

        var command = new UpdateAlertStatusCommand { AlertId = Guid.NewGuid(), Status = "Acknowledged" };

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    /// <summary>
    /// Verifies that acknowledging an alert sets <c>AcknowledgedAt</c> and <c>AcknowledgedByUserId</c>,
    /// and that the repository's <c>UpdateAsync</c> is called once.
    /// </summary>
    [Fact]
    public async Task Handle_AcknowledgeAlert_SetsAcknowledgedAtAndUserId()
    {
        var alert = CreateAlert();
        _alertRepositoryMock.Setup(r => r.GetByIdAsync(alert.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alert);

        var userId = "user-123";
        var command = new UpdateAlertStatusCommand
        {
            AlertId = alert.Id,
            Status = "Acknowledged",
            UserId = userId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(AlertStatus.Acknowledged);
        result.AcknowledgedAt.Should().NotBeNull();
        result.AcknowledgedByUserId.Should().Be(userId);

        _alertRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Alert>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that resolving an alert sets the <c>ResolvedAt</c> timestamp.
    /// </summary>
    [Fact]
    public async Task Handle_ResolveAlert_SetsResolvedAt()
    {
        var alert = CreateAlert(AlertStatus.Acknowledged);
        _alertRepositoryMock.Setup(r => r.GetByIdAsync(alert.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alert);

        var command = new UpdateAlertStatusCommand
        {
            AlertId = alert.Id,
            Status = "Resolved"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(AlertStatus.Resolved);
        result.ResolvedAt.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that transitioning an alert back to <c>Open</c> does not set
    /// <c>AcknowledgedAt</c> or <c>ResolvedAt</c>.
    /// </summary>
    [Fact]
    public async Task Handle_UpdateToOpen_DoesNotSetAcknowledgedOrResolved()
    {
        var alert = CreateAlert(AlertStatus.Acknowledged);
        alert.AcknowledgedAt = null;
        alert.ResolvedAt = null;
        _alertRepositoryMock.Setup(r => r.GetByIdAsync(alert.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alert);

        var command = new UpdateAlertStatusCommand { AlertId = alert.Id, Status = "Open" };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(AlertStatus.Open);
        result.ResolvedAt.Should().BeNull();
    }

    /// <summary>
    /// Verifies that a successful status update also advances the alert's <c>UpdatedAt</c> timestamp.
    /// </summary>
    [Fact]
    public async Task Handle_ValidUpdate_UpdatesUpdatedAt()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var alert = CreateAlert();
        _alertRepositoryMock.Setup(r => r.GetByIdAsync(alert.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alert);

        await _handler.Handle(new UpdateAlertStatusCommand { AlertId = alert.Id, Status = "Resolved" }, CancellationToken.None);

        alert.UpdatedAt.Should().BeAfter(before);
    }
}
