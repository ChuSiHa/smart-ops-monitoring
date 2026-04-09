using FluentAssertions;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Domain.Events;

namespace SmartOpsMonitoring.Tests.Domain;

public class DomainEventTests
{
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

    [Theory]
    [InlineData(AlertSeverity.Info)]
    [InlineData(AlertSeverity.Warning)]
    [InlineData(AlertSeverity.Critical)]
    public void AlertCreatedEvent_AllSeverities_AreSupportedInConstructor(AlertSeverity severity)
    {
        var evt = new AlertCreatedEvent(Guid.NewGuid(), severity, Guid.NewGuid());

        evt.Severity.Should().Be(severity);
    }

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

    [Fact]
    public void MetricReceivedEvent_NegativeValue_IsAllowed()
    {
        var evt = new MetricReceivedEvent(Guid.NewGuid(), Guid.NewGuid(), "temp", -10.5);

        evt.Value.Should().Be(-10.5);
    }

    [Fact]
    public void MetricReceivedEvent_ZeroValue_IsAllowed()
    {
        var evt = new MetricReceivedEvent(Guid.NewGuid(), Guid.NewGuid(), "disk_io", 0.0);

        evt.Value.Should().Be(0.0);
    }
}
