using FluentAssertions;
using SmartOpsMonitoring.Application.Features.Metrics.Commands.IngestMetric;

namespace SmartOpsMonitoring.Tests.Application.Validators;

/// <summary>
/// Unit tests for <see cref="IngestMetricCommandValidator"/>.
/// </summary>
public class IngestMetricCommandValidatorTests
{
    private readonly IngestMetricCommandValidator _validator = new();

    /// <summary>Returns a fully populated, valid <see cref="IngestMetricCommand"/> for use as a baseline.</summary>
    private static IngestMetricCommand ValidCommand() => new()
    {
        HostId = Guid.NewGuid(),
        MetricType = "cpu_usage",
        Value = 75.0,
        Unit = "percent"
    };

    /// <summary>
    /// Verifies that a fully populated, valid command passes all validation rules.
    /// </summary>
    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var result = await _validator.ValidateAsync(ValidCommand());
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that an empty <c>HostId</c> produces a validation error on the <c>HostId</c> field.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyHostId_Fails()
    {
        var cmd = ValidCommand();
        cmd.HostId = Guid.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.HostId));
    }

    /// <summary>
    /// Verifies that an empty <c>MetricType</c> produces a validation error on the <c>MetricType</c> field.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyMetricType_Fails()
    {
        var cmd = ValidCommand();
        cmd.MetricType = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.MetricType));
    }

    /// <summary>
    /// Verifies that a <c>MetricType</c> exceeding the 100-character maximum fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_MetricTypeExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.MetricType = new string('m', 101);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.MetricType));
    }

    /// <summary>
    /// Verifies that an empty <c>Unit</c> produces a validation error on the <c>Unit</c> field.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyUnit_Fails()
    {
        var cmd = ValidCommand();
        cmd.Unit = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Unit));
    }

    /// <summary>
    /// Verifies that a <c>Unit</c> exceeding the 50-character maximum fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_UnitExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Unit = new string('u', 51);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Unit));
    }

    /// <summary>
    /// Verifies that <c>double.NaN</c> as the metric <c>Value</c> fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_NaNValue_Fails()
    {
        var cmd = ValidCommand();
        cmd.Value = double.NaN;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Value));
    }

    /// <summary>
    /// Verifies that <c>double.PositiveInfinity</c> as the metric <c>Value</c> fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_PositiveInfinityValue_Fails()
    {
        var cmd = ValidCommand();
        cmd.Value = double.PositiveInfinity;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Value));
    }

    /// <summary>
    /// Verifies that <c>double.NegativeInfinity</c> as the metric <c>Value</c> fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_NegativeInfinityValue_Fails()
    {
        var cmd = ValidCommand();
        cmd.Value = double.NegativeInfinity;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Value));
    }

    /// <summary>
    /// Verifies that any finite double value (zero, negative, large positive) passes validation.
    /// </summary>
    [Theory]
    [InlineData(0.0)]
    [InlineData(-100.5)]
    [InlineData(9999.99)]
    public async Task Validate_FiniteValue_Passes(double value)
    {
        var cmd = ValidCommand();
        cmd.Value = value;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that providing an optional <c>Timestamp</c> does not fail validation.
    /// </summary>
    [Fact]
    public async Task Validate_WithOptionalTimestamp_Passes()
    {
        var cmd = ValidCommand();
        cmd.Timestamp = DateTime.UtcNow;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that providing an optional <c>ServiceNodeId</c> does not fail validation.
    /// </summary>
    [Fact]
    public async Task Validate_WithOptionalServiceNodeId_Passes()
    {
        var cmd = ValidCommand();
        cmd.ServiceNodeId = Guid.NewGuid();
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }
}
