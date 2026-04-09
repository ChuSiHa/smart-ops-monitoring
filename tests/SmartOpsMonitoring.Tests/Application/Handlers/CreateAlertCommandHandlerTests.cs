using FluentAssertions;
using Moq;
using SmartOpsMonitoring.Application.Features.Alerts.Commands.CreateAlert;
using SmartOpsMonitoring.Application.Mappings;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Domain.Events;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Tests.Application.Handlers;

/// <summary>
/// Unit tests for <see cref="CreateAlertCommandHandler"/>.
/// </summary>
public class CreateAlertCommandHandlerTests
{
    private readonly Mock<IAlertRepository> _alertRepositoryMock = new();
    private readonly Mock<MediatR.IPublisher> _publisherMock = new();
    private readonly CreateAlertCommandHandler _handler;

    /// <summary>Initialises mocks and registers Mapster mappings required by the handler.</summary>
    public CreateAlertCommandHandlerTests()
    {
        MappingConfig.RegisterMappings();
        _handler = new CreateAlertCommandHandler(_alertRepositoryMock.Object, _publisherMock.Object);
    }

    /// <summary>
    /// Verifies that handling a valid command persists the alert to the repository and
    /// publishes an <see cref="AlertCreatedEvent"/> with the correct host and severity.
    /// </summary>
    [Fact]
    public async Task Handle_ValidCommand_AddsAlertAndPublishesEvent()
    {
        var hostId = Guid.NewGuid();
        var command = new CreateAlertCommand
        {
            HostId = hostId,
            Title = "High Memory",
            Message = "Memory usage above 95%",
            Severity = "Critical"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.HostId.Should().Be(hostId);
        result.Title.Should().Be("High Memory");
        result.Severity.Should().Be(AlertSeverity.Critical);
        result.Status.Should().Be(AlertStatus.Open);

        _alertRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Alert>(), It.IsAny<CancellationToken>()), Times.Once);
        _publisherMock.Verify(p => p.Publish(
            It.Is<AlertCreatedEvent>(e => e.HostId == hostId && e.Severity == AlertSeverity.Critical),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that when a <c>ServiceNodeId</c> is provided on the command, it is reflected
    /// in the returned <see cref="AlertDto"/>.
    /// </summary>
    [Fact]
    public async Task Handle_WithServiceNodeId_IncludesServiceNodeId()
    {
        var serviceNodeId = Guid.NewGuid();
        var command = new CreateAlertCommand
        {
            HostId = Guid.NewGuid(),
            ServiceNodeId = serviceNodeId,
            Title = "Service Down",
            Message = "Service is not responding",
            Severity = "Warning"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ServiceNodeId.Should().Be(serviceNodeId);
    }

    /// <summary>
    /// Verifies that all three severity strings ("Info", "Warning", "Critical") are correctly
    /// mapped to their corresponding <see cref="AlertSeverity"/> enum values.
    /// </summary>
    [Theory]
    [InlineData("Info", AlertSeverity.Info)]
    [InlineData("Warning", AlertSeverity.Warning)]
    [InlineData("Critical", AlertSeverity.Critical)]
    public async Task Handle_AllSeverityLevels_MapCorrectly(string severityStr, AlertSeverity expectedSeverity)
    {
        var command = new CreateAlertCommand
        {
            HostId = Guid.NewGuid(),
            Title = "Test Alert",
            Message = "Test message",
            Severity = severityStr
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Severity.Should().Be(expectedSeverity);
    }

    /// <summary>
    /// Verifies that a newly created alert always has <see cref="AlertStatus.Open"/> status,
    /// regardless of what was supplied on the command.
    /// </summary>
    [Fact]
    public async Task Handle_NewAlert_HasOpenStatus()
    {
        var command = new CreateAlertCommand
        {
            HostId = Guid.NewGuid(),
            Title = "Test",
            Message = "Test message",
            Severity = "Info"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(AlertStatus.Open);
    }
}
