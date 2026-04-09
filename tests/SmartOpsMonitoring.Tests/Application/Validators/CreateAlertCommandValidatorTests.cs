using FluentAssertions;
using SmartOpsMonitoring.Application.Features.Alerts.Commands.CreateAlert;

namespace SmartOpsMonitoring.Tests.Application.Validators;

public class CreateAlertCommandValidatorTests
{
    private readonly CreateAlertCommandValidator _validator = new();

    private static CreateAlertCommand ValidCommand() => new()
    {
        HostId = Guid.NewGuid(),
        Title = "High CPU",
        Message = "CPU usage exceeded 90%",
        Severity = "Warning"
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
    public async Task Validate_EmptyTitle_Fails()
    {
        var cmd = ValidCommand();
        cmd.Title = string.Empty;

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Title));
    }

    [Fact]
    public async Task Validate_TitleExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Title = new string('A', 201);

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Title));
    }

    [Fact]
    public async Task Validate_EmptyMessage_Fails()
    {
        var cmd = ValidCommand();
        cmd.Message = string.Empty;

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Message));
    }

    [Fact]
    public async Task Validate_MessageExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Message = new string('M', 2001);

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Message));
    }

    [Theory]
    [InlineData("Info")]
    [InlineData("Warning")]
    [InlineData("Critical")]
    [InlineData("info")]
    [InlineData("WARNING")]
    public async Task Validate_ValidSeverity_Passes(string severity)
    {
        var cmd = ValidCommand();
        cmd.Severity = severity;

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Unknown")]
    [InlineData("High")]
    [InlineData("low")]
    public async Task Validate_InvalidSeverity_Fails(string severity)
    {
        var cmd = ValidCommand();
        cmd.Severity = severity;

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Severity));
    }

    [Fact]
    public async Task Validate_OptionalServiceNodeId_IsAllowedNull()
    {
        var cmd = ValidCommand();
        cmd.ServiceNodeId = null;

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithServiceNodeId_Passes()
    {
        var cmd = ValidCommand();
        cmd.ServiceNodeId = Guid.NewGuid();

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeTrue();
    }
}
