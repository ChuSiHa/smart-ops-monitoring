using FluentAssertions;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Domain.Events;

namespace SmartOpsMonitoring.Tests.Domain;

/// <summary>
/// Unit tests for domain events: <see cref="AlertCreatedEvent"/> and <see cref="MetricReceivedEvent"/>.
/// </summary>
public class DomainEventTests
{
    /// <summary>
    /// Verifies that the <see cref="AlertCreatedEvent"/> constructor correctly stores
    /// the alert identifier, severity, and host identifier.
    /// </summary>
    [Fact]
    public void AlertCreatedEvent_Constructor_SetsProperties()
    {
        var alertId = Guid.NewGuid();
        var hostId = Guid.NewGuid();
        var severity = AlertSeverity.Critical;

        var evt = new AlertCreatedEvent(alertId, severity, hostId);

        evt.AlertId.Should().Be(alertId);
        evt.Severity.Should().Be(severity);
        evt.HostId.Should().Be(hostId);
    }

    /// <summary>
    /// Verifies that all <see cref="AlertSeverity"/> values are accepted by the
    /// <see cref="AlertCreatedEvent"/> constructor without throwing.
    /// </summary>
    [Theory]
    [InlineData(AlertSeverity.Info)]
    [InlineData(AlertSeverity.Warning)]
    [InlineData(AlertSeverity.Critical)]
    public void AlertCreatedEvent_AllSeverities_AreSupportedInConstructor(AlertSeverity severity)
    {
        var evt = new AlertCreatedEvent(Guid.NewGuid(), severity, Guid.NewGuid());

        evt.Severity.Should().Be(severity);
    }

    /// <summary>
    /// Verifies that the <see cref="MetricReceivedEvent"/> constructor correctly stores
    /// the metric identifier, host identifier, metric type, and numeric value.
    /// </summary>
    [Fact]
    public void MetricReceivedEvent_Constructor_SetsProperties()
    {
        var metricId = Guid.NewGuid();
        var hostId = Guid.NewGuid();
        const string metricType = "cpu_usage";
        const double value = 75.5;

        var evt = new MetricReceivedEvent(metricId, hostId, metricType, value);

        evt.MetricId.Should().Be(metricId);
        evt.HostId.Should().Be(hostId);
        evt.MetricType.Should().Be(metricType);
        evt.Value.Should().Be(value);
    }

    /// <summary>
    /// Verifies that a negative metric value is accepted by the <see cref="MetricReceivedEvent"/> constructor.
    /// </summary>
    [Fact]
    public void MetricReceivedEvent_NegativeValue_IsAllowed()
    {
        var evt = new MetricReceivedEvent(Guid.NewGuid(), Guid.NewGuid(), "temp", -10.5);

        evt.Value.Should().Be(-10.5);
    }

    /// <summary>
    /// Verifies that a zero metric value is accepted by the <see cref="MetricReceivedEvent"/> constructor.
    /// </summary>
    [Fact]
    public void MetricReceivedEvent_ZeroValue_IsAllowed()
    {
        var evt = new MetricReceivedEvent(Guid.NewGuid(), Guid.NewGuid(), "disk_io", 0.0);

        evt.Value.Should().Be(0.0);
    }
}
