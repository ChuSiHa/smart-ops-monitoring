using FluentAssertions;
using SmartOpsMonitoring.Application.Features.Metrics.Commands.IngestMetric;

namespace SmartOpsMonitoring.Tests.Application.Validators;

public class IngestMetricCommandValidatorTests
{
    private readonly IngestMetricCommandValidator _validator = new();

    private static IngestMetricCommand ValidCommand() => new()
    {
        HostId = Guid.NewGuid(),
        MetricType = "cpu_usage",
        Value = 75.0,
        Unit = "percent"
    };

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var result = await _validator.ValidateAsync(ValidCommand());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyHostId_Fails()
    {
        var cmd = ValidCommand();
        cmd.HostId = Guid.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.HostId));
    }

    [Fact]
    public async Task Validate_EmptyMetricType_Fails()
    {
        var cmd = ValidCommand();
        cmd.MetricType = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.MetricType));
    }

    [Fact]
    public async Task Validate_MetricTypeExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.MetricType = new string('m', 101);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.MetricType));
    }

    [Fact]
    public async Task Validate_EmptyUnit_Fails()
    {
        var cmd = ValidCommand();
        cmd.Unit = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Unit));
    }

    [Fact]
    public async Task Validate_UnitExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Unit = new string('u', 51);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Unit));
    }

    [Fact]
    public async Task Validate_NaNValue_Fails()
    {
        var cmd = ValidCommand();
        cmd.Value = double.NaN;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Value));
    }

    [Fact]
    public async Task Validate_PositiveInfinityValue_Fails()
    {
        var cmd = ValidCommand();
        cmd.Value = double.PositiveInfinity;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Value));
    }

    [Fact]
    public async Task Validate_NegativeInfinityValue_Fails()
    {
        var cmd = ValidCommand();
        cmd.Value = double.NegativeInfinity;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Value));
    }

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

    [Fact]
    public async Task Validate_WithOptionalTimestamp_Passes()
    {
        var cmd = ValidCommand();
        cmd.Timestamp = DateTime.UtcNow;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithOptionalServiceNodeId_Passes()
    {
        var cmd = ValidCommand();
        cmd.ServiceNodeId = Guid.NewGuid();
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }
}
