using FluentAssertions;
using Moq;
using SmartOpsMonitoring.Application.Features.Metrics.Commands.IngestMetric;
using SmartOpsMonitoring.Application.Features.Metrics.Queries.GetMetrics;
using SmartOpsMonitoring.Application.Mappings;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Events;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Tests.Application.Handlers;

public class MetricHandlerTests
{
    private readonly Mock<IMetricRepository> _metricRepositoryMock = new();
    private readonly Mock<MediatR.IPublisher> _publisherMock = new();

    public MetricHandlerTests()
    {
        MappingConfig.RegisterMappings();
    }

    // --- IngestMetricCommandHandler ---

    [Fact]
    public async Task IngestMetricHandler_ValidCommand_AddsMetricAndPublishesEvent()
    {
        var handler = new IngestMetricCommandHandler(_metricRepositoryMock.Object, _publisherMock.Object);
        var hostId = Guid.NewGuid();
        var command = new IngestMetricCommand
        {
            HostId = hostId,
            MetricType = "cpu_usage",
            Value = 80.5,
            Unit = "percent"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.HostId.Should().Be(hostId);
        result.MetricType.Should().Be("cpu_usage");
        result.Value.Should().Be(80.5);
        result.Unit.Should().Be("percent");

        _metricRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Metric>(), It.IsAny<CancellationToken>()), Times.Once);
        _publisherMock.Verify(p => p.Publish(
            It.Is<MetricReceivedEvent>(e => e.HostId == hostId && e.MetricType == "cpu_usage"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IngestMetricHandler_WithTimestamp_PreservesTimestamp()
    {
        var handler = new IngestMetricCommandHandler(_metricRepositoryMock.Object, _publisherMock.Object);
        var customTimestamp = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var command = new IngestMetricCommand
        {
            HostId = Guid.NewGuid(),
            MetricType = "memory_bytes",
            Value = 1024,
            Unit = "bytes",
            Timestamp = customTimestamp
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Timestamp.Should().Be(customTimestamp);
    }

    [Fact]
    public async Task IngestMetricHandler_WithoutTimestamp_DefaultsToUtcNow()
    {
        var handler = new IngestMetricCommandHandler(_metricRepositoryMock.Object, _publisherMock.Object);
        var before = DateTime.UtcNow.AddSeconds(-1);
        var command = new IngestMetricCommand
        {
            HostId = Guid.NewGuid(),
            MetricType = "disk_usage",
            Value = 50,
            Unit = "percent",
            Timestamp = null
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Timestamp.Should().BeAfter(before).And.BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task IngestMetricHandler_WithLabels_PreservesLabels()
    {
        var handler = new IngestMetricCommandHandler(_metricRepositoryMock.Object, _publisherMock.Object);
        var labels = new Dictionary<string, string> { { "env", "prod" }, { "region", "us-east-1" } };
        var command = new IngestMetricCommand
        {
            HostId = Guid.NewGuid(),
            MetricType = "request_count",
            Value = 100,
            Unit = "count",
            Labels = labels
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Labels.Should().ContainKey("env").WhoseValue.Should().Be("prod");
    }

    // --- GetMetricsByHostQueryHandler ---

    [Fact]
    public async Task GetMetricsByHostHandler_NoTypeOrRange_CallsGetByHostId()
    {
        var hostId = Guid.NewGuid();
        var metrics = new List<Metric>
        {
            new() { Id = Guid.NewGuid(), HostId = hostId, MetricType = "cpu_usage", Value = 70, Unit = "percent" }
        };
        _metricRepositoryMock.Setup(r => r.GetByHostIdAsync(hostId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(metrics);

        var handler = new GetMetricsByHostQueryHandler(_metricRepositoryMock.Object);
        var result = await handler.Handle(new GetMetricsByHostQuery { HostId = hostId }, CancellationToken.None);

        result.Should().HaveCount(1);
        _metricRepositoryMock.Verify(r => r.GetByHostIdAsync(hostId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMetricsByHostHandler_WithTypeAndRange_CallsGetByTypeAndRange()
    {
        var hostId = Guid.NewGuid();
        var from = DateTime.UtcNow.AddHours(-1);
        var to = DateTime.UtcNow;
        var metrics = new List<Metric>
        {
            new() { Id = Guid.NewGuid(), HostId = hostId, MetricType = "cpu_usage", Value = 60, Unit = "percent" }
        };
        _metricRepositoryMock.Setup(r => r.GetByTypeAndRangeAsync(hostId, "cpu_usage", from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync(metrics);

        var handler = new GetMetricsByHostQueryHandler(_metricRepositoryMock.Object);
        var result = await handler.Handle(new GetMetricsByHostQuery
        {
            HostId = hostId,
            MetricType = "cpu_usage",
            From = from,
            To = to
        }, CancellationToken.None);

        result.Should().HaveCount(1);
        _metricRepositoryMock.Verify(
            r => r.GetByTypeAndRangeAsync(hostId, "cpu_usage", from, to, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMetricsByHostHandler_TypeWithoutRange_FallsBackToGetByHostId()
    {
        var hostId = Guid.NewGuid();
        _metricRepositoryMock.Setup(r => r.GetByHostIdAsync(hostId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Metric>());

        var handler = new GetMetricsByHostQueryHandler(_metricRepositoryMock.Object);
        await handler.Handle(new GetMetricsByHostQuery { HostId = hostId, MetricType = "cpu_usage" }, CancellationToken.None);

        _metricRepositoryMock.Verify(r => r.GetByHostIdAsync(hostId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
