using FluentAssertions;
using SmartOpsMonitoring.Domain.ValueObjects;

namespace SmartOpsMonitoring.Tests.Domain;

/// <summary>
/// Unit tests for the <see cref="MetricLabel"/> value object.
/// </summary>
public class MetricLabelTests
{
    /// <summary>
    /// Verifies that the constructor correctly assigns the <c>Key</c> and <c>Value</c> properties.
    /// </summary>
    [Fact]
    public void MetricLabel_Constructor_SetsKeyAndValue()
    {
        var label = new MetricLabel("env", "production");

        label.Key.Should().Be("env");
        label.Value.Should().Be("production");
    }

    /// <summary>
    /// Verifies that two <see cref="MetricLabel"/> instances with the same key and value
    /// are considered equal (record value semantics).
    /// </summary>
    [Fact]
    public void MetricLabel_SameKeyAndValue_AreEqual()
    {
        var label1 = new MetricLabel("region", "us-east-1");
        var label2 = new MetricLabel("region", "us-east-1");

        label1.Should().Be(label2);
    }

    /// <summary>
    /// Verifies that two <see cref="MetricLabel"/> instances with different keys are not equal.
    /// </summary>
    [Fact]
    public void MetricLabel_DifferentKey_NotEqual()
    {
        var label1 = new MetricLabel("env", "prod");
        var label2 = new MetricLabel("stage", "prod");

        label1.Should().NotBe(label2);
    }

    /// <summary>
    /// Verifies that two <see cref="MetricLabel"/> instances with different values are not equal.
    /// </summary>
    [Fact]
    public void MetricLabel_DifferentValue_NotEqual()
    {
        var label1 = new MetricLabel("env", "prod");
        var label2 = new MetricLabel("env", "staging");

        label1.Should().NotBe(label2);
    }
}
