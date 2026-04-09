using FluentAssertions;
using SmartOpsMonitoring.Application.Features.Alerts.Commands.UpdateAlertStatus;

namespace SmartOpsMonitoring.Tests.Application.Validators;

public class UpdateAlertStatusCommandValidatorTests
{
    private readonly UpdateAlertStatusCommandValidator _validator = new();

    private static UpdateAlertStatusCommand ValidCommand() => new()
    {
        AlertId = Guid.NewGuid(),
        Status = "Acknowledged"
    };

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var result = await _validator.ValidateAsync(ValidCommand());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyAlertId_Fails()
    {
        var cmd = ValidCommand();
        cmd.AlertId = Guid.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.AlertId));
    }

    [Fact]
    public async Task Validate_EmptyStatus_Fails()
    {
        var cmd = ValidCommand();
        cmd.Status = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Status));
    }

    [Theory]
    [InlineData("Open")]
    [InlineData("Acknowledged")]
    [InlineData("Resolved")]
    [InlineData("open")]
    [InlineData("RESOLVED")]
    public async Task Validate_ValidStatus_Passes(string status)
    {
        var cmd = ValidCommand();
        cmd.Status = status;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("Closed")]
    [InlineData("Pending")]
    [InlineData("Invalid")]
    public async Task Validate_InvalidStatus_Fails(string status)
    {
        var cmd = ValidCommand();
        cmd.Status = status;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Status));
    }

    [Fact]
    public async Task Validate_StatusExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Status = new string('A', 51);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_NullUserId_IsAllowed()
    {
        var cmd = ValidCommand();
        cmd.UserId = null;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }
}
