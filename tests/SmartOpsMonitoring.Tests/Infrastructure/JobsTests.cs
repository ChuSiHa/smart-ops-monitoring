using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SmartOpsMonitoring.Infrastructure.Jobs;

namespace SmartOpsMonitoring.Tests.Infrastructure;

/// <summary>
/// Unit tests for Hangfire background jobs:
/// <see cref="HealthCheckPollingJob"/>, <see cref="MetricAggregationJob"/>,
/// and <see cref="StaleAlertCleanupJob"/>.
/// </summary>
public class JobsTests
{
    // --- HealthCheckPollingJob ---

    /// <summary>
    /// Verifies that <see cref="HealthCheckPollingJob.ExecuteAsync"/> completes successfully
    /// without throwing any exceptions.
    /// </summary>
    [Fact]
    public async Task HealthCheckPollingJob_Execute_CompletesWithoutError()
    {
        var loggerMock = new Mock<ILogger<HealthCheckPollingJob>>();
        var job = new HealthCheckPollingJob(loggerMock.Object);

        var act = async () => await job.ExecuteAsync();

        await act.Should().NotThrowAsync();
    }

    /// <summary>
    /// Verifies that <see cref="HealthCheckPollingJob.ExecuteAsync"/> logs at least one
    /// <see cref="LogLevel.Information"/> message when it executes.
    /// </summary>
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

    /// <summary>
    /// Verifies that <see cref="MetricAggregationJob.ExecuteAsync"/> completes successfully
    /// without throwing any exceptions.
    /// </summary>
    [Fact]
    public async Task MetricAggregationJob_Execute_CompletesWithoutError()
    {
        var loggerMock = new Mock<ILogger<MetricAggregationJob>>();
        var job = new MetricAggregationJob(loggerMock.Object);

        var act = async () => await job.ExecuteAsync();

        await act.Should().NotThrowAsync();
    }

    /// <summary>
    /// Verifies that <see cref="MetricAggregationJob.ExecuteAsync"/> logs at least one
    /// <see cref="LogLevel.Information"/> message when it executes.
    /// </summary>
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

    /// <summary>
    /// Verifies that <see cref="StaleAlertCleanupJob.ExecuteAsync"/> completes successfully
    /// without throwing any exceptions.
    /// </summary>
    [Fact]
    public async Task StaleAlertCleanupJob_Execute_CompletesWithoutError()
    {
        var loggerMock = new Mock<ILogger<StaleAlertCleanupJob>>();
        var job = new StaleAlertCleanupJob(loggerMock.Object);

        var act = async () => await job.ExecuteAsync();

        await act.Should().NotThrowAsync();
    }

    /// <summary>
    /// Verifies that <see cref="StaleAlertCleanupJob.ExecuteAsync"/> logs at least one
    /// <see cref="LogLevel.Information"/> message when it executes.
    /// </summary>
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
