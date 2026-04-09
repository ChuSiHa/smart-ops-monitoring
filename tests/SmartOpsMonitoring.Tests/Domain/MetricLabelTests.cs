using FluentAssertions;
using SmartOpsMonitoring.Domain.ValueObjects;

namespace SmartOpsMonitoring.Tests.Domain;

public class MetricLabelTests
{
    [Fact]
    public void MetricLabel_Constructor_SetsKeyAndValue()
    {
        var label = new MetricLabel("env", "production");

        label.Key.Should().Be("env");
        label.Value.Should().Be("production");
    }

    [Fact]
    public void MetricLabel_SameKeyAndValue_AreEqual()
    {
        var label1 = new MetricLabel("region", "us-east-1");
        var label2 = new MetricLabel("region", "us-east-1");

        label1.Should().Be(label2);
    }

    [Fact]
    public void MetricLabel_DifferentKey_NotEqual()
    {
        var label1 = new MetricLabel("env", "prod");
        var label2 = new MetricLabel("stage", "prod");

        label1.Should().NotBe(label2);
    }

    [Fact]
    public void MetricLabel_DifferentValue_NotEqual()
    {
        var label1 = new MetricLabel("env", "prod");
        var label2 = new MetricLabel("env", "staging");

        label1.Should().NotBe(label2);
    }
}
