using FluentAssertions;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Enums;

namespace SmartOpsMonitoring.Tests.Domain;

/// <summary>
/// Unit tests for domain entity default values and <see cref="Domain.Entities.BaseEntity"/> behaviour.
/// </summary>
public class EntityTests
{
    /// <summary>
    /// Verifies that a newly constructed entity has a non-empty GUID identifier.
    /// </summary>
    [Fact]
    public void BaseEntity_DefaultId_IsNotEmpty()
    {
        var host = new Host();

        host.Id.Should().NotBeEmpty();
    }

    /// <summary>
    /// Verifies that <c>CreatedAt</c> is set to a recent UTC timestamp upon construction.
    /// </summary>
    [Fact]
    public void BaseEntity_CreatedAt_IsSetOnCreation()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var host = new Host();
        var after = DateTime.UtcNow.AddSeconds(1);

        host.CreatedAt.Should().BeAfter(before).And.BeBefore(after);
    }

    /// <summary>
    /// Verifies that <c>UpdatedAt</c> is set to a recent UTC timestamp upon construction.
    /// </summary>
    [Fact]
    public void BaseEntity_UpdatedAt_IsSetOnCreation()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var host = new Host();
        var after = DateTime.UtcNow.AddSeconds(1);

        host.UpdatedAt.Should().BeAfter(before).And.BeBefore(after);
    }

    /// <summary>
    /// Verifies that a new <see cref="Host"/> defaults to <see cref="HostStatus.Unknown"/>.
    /// </summary>
    [Fact]
    public void Host_DefaultStatus_IsUnknown()
    {
        var host = new Host();

        host.Status.Should().Be(HostStatus.Unknown);
    }

    /// <summary>
    /// Verifies that the <c>Tags</c> collection is initialised as an empty list on a new <see cref="Host"/>.
    /// </summary>
    [Fact]
    public void Host_DefaultTags_IsEmptyList()
    {
        var host = new Host();

        host.Tags.Should().NotBeNull().And.BeEmpty();
    }

    /// <summary>
    /// Verifies that the <c>ServiceNodes</c> navigation property is initialised as an empty list on a new <see cref="Host"/>.
    /// </summary>
    [Fact]
    public void Host_DefaultServiceNodes_IsEmptyList()
    {
        var host = new Host();

        host.ServiceNodes.Should().NotBeNull().And.BeEmpty();
    }

    /// <summary>
    /// Verifies that a new <see cref="Alert"/> defaults to <see cref="AlertStatus.Open"/>.
    /// </summary>
    [Fact]
    public void Alert_DefaultStatus_IsOpen()
    {
        var alert = new Alert();

        alert.Status.Should().Be(AlertStatus.Open);
    }

    /// <summary>
    /// Verifies that a new <see cref="Alert"/> defaults to <see cref="AlertSeverity.Info"/>.
    /// </summary>
    [Fact]
    public void Alert_DefaultSeverity_IsInfo()
    {
        var alert = new Alert();

        alert.Severity.Should().Be(AlertSeverity.Info);
    }

    /// <summary>
    /// Verifies that acknowledgement and resolution fields are null on a newly created <see cref="Alert"/>.
    /// </summary>
    [Fact]
    public void Alert_AcknowledgedAt_NullByDefault()
    {
        var alert = new Alert();

        alert.AcknowledgedAt.Should().BeNull();
        alert.ResolvedAt.Should().BeNull();
        alert.AcknowledgedByUserId.Should().BeNull();
    }

    /// <summary>
    /// Verifies that a new <see cref="ServiceNode"/> defaults to <see cref="ServiceNodeStatus.Unknown"/>.
    /// </summary>
    [Fact]
    public void ServiceNode_DefaultStatus_IsUnknown()
    {
        var node = new ServiceNode();

        node.Status.Should().Be(ServiceNodeStatus.Unknown);
    }

    /// <summary>
    /// Verifies that a new <see cref="Metric"/> has its <c>Timestamp</c> set to a recent UTC value.
    /// </summary>
    [Fact]
    public void Metric_DefaultTimestamp_IsRecent()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var metric = new Metric();
        var after = DateTime.UtcNow.AddSeconds(1);

        metric.Timestamp.Should().BeAfter(before).And.BeBefore(after);
    }

    /// <summary>
    /// Verifies that the <c>Labels</c> dictionary is initialised as empty on a new <see cref="Metric"/>.
    /// </summary>
    [Fact]
    public void Metric_DefaultLabels_IsEmptyDictionary()
    {
        var metric = new Metric();

        metric.Labels.Should().NotBeNull().And.BeEmpty();
    }
}
