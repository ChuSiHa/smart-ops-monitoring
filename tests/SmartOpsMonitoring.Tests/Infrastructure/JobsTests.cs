using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SmartOpsMonitoring.Infrastructure.Jobs;

namespace SmartOpsMonitoring.Tests.Infrastructure;

public class JobsTests
{
    // --- HealthCheckPollingJob ---

    [Fact]
    public async Task HealthCheckPollingJob_Execute_CompletesWithoutError()
    {
        var loggerMock = new Mock<ILogger<HealthCheckPollingJob>>();
        var job = new HealthCheckPollingJob(loggerMock.Object);

        var act = async () => await job.ExecuteAsync();

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HealthCheckPollingJob_Execute_LogsInformation()
    {
        var loggerMock = new Mock<ILogger<HealthCheckPollingJob>>();
        var job = new HealthCheckPollingJob(loggerMock.Object);

        await job.ExecuteAsync();

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    // --- MetricAggregationJob ---

    [Fact]
    public async Task MetricAggregationJob_Execute_CompletesWithoutError()
    {
        var loggerMock = new Mock<ILogger<MetricAggregationJob>>();
        var job = new MetricAggregationJob(loggerMock.Object);

        var act = async () => await job.ExecuteAsync();

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task MetricAggregationJob_Execute_LogsInformation()
    {
        var loggerMock = new Mock<ILogger<MetricAggregationJob>>();
        var job = new MetricAggregationJob(loggerMock.Object);

        await job.ExecuteAsync();

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    // --- StaleAlertCleanupJob ---

    [Fact]
    public async Task StaleAlertCleanupJob_Execute_CompletesWithoutError()
    {
        var loggerMock = new Mock<ILogger<StaleAlertCleanupJob>>();
        var job = new StaleAlertCleanupJob(loggerMock.Object);

        var act = async () => await job.ExecuteAsync();

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task StaleAlertCleanupJob_Execute_LogsInformation()
    {
        var loggerMock = new Mock<ILogger<StaleAlertCleanupJob>>();
        var job = new StaleAlertCleanupJob(loggerMock.Object);

        await job.ExecuteAsync();

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
