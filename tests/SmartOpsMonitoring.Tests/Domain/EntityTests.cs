using FluentAssertions;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Enums;

namespace SmartOpsMonitoring.Tests.Domain;

public class EntityTests
{
    [Fact]
    public void BaseEntity_DefaultId_IsNotEmpty()
    {
        var host = new Host();

        host.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void BaseEntity_CreatedAt_IsSetOnCreation()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var host = new Host();
        var after = DateTime.UtcNow.AddSeconds(1);

        host.CreatedAt.Should().BeAfter(before).And.BeBefore(after);
    }

    [Fact]
    public void BaseEntity_UpdatedAt_IsSetOnCreation()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var host = new Host();
        var after = DateTime.UtcNow.AddSeconds(1);

        host.UpdatedAt.Should().BeAfter(before).And.BeBefore(after);
    }

    [Fact]
    public void Host_DefaultStatus_IsUnknown()
    {
        var host = new Host();

        host.Status.Should().Be(HostStatus.Unknown);
    }

    [Fact]
    public void Host_DefaultTags_IsEmptyList()
    {
        var host = new Host();

        host.Tags.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Host_DefaultServiceNodes_IsEmptyList()
    {
        var host = new Host();

        host.ServiceNodes.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Alert_DefaultStatus_IsOpen()
    {
        var alert = new Alert();

        alert.Status.Should().Be(AlertStatus.Open);
    }

    [Fact]
    public void Alert_DefaultSeverity_IsInfo()
    {
        var alert = new Alert();

        alert.Severity.Should().Be(AlertSeverity.Info);
    }

    [Fact]
    public void Alert_AcknowledgedAt_NullByDefault()
    {
        var alert = new Alert();

        alert.AcknowledgedAt.Should().BeNull();
        alert.ResolvedAt.Should().BeNull();
        alert.AcknowledgedByUserId.Should().BeNull();
    }

    [Fact]
    public void ServiceNode_DefaultStatus_IsUnknown()
    {
        var node = new ServiceNode();

        node.Status.Should().Be(ServiceNodeStatus.Unknown);
    }

    [Fact]
    public void Metric_DefaultTimestamp_IsRecent()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var metric = new Metric();
        var after = DateTime.UtcNow.AddSeconds(1);

        metric.Timestamp.Should().BeAfter(before).And.BeBefore(after);
    }

    [Fact]
    public void Metric_DefaultLabels_IsEmptyDictionary()
    {
        var metric = new Metric();

        metric.Labels.Should().NotBeNull().And.BeEmpty();
    }
}
