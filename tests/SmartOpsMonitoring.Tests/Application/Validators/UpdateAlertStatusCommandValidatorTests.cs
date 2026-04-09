using FluentAssertions;
using SmartOpsMonitoring.Application.Features.Alerts.Commands.UpdateAlertStatus;

namespace SmartOpsMonitoring.Tests.Application.Validators;

/// <summary>
/// Unit tests for <see cref="UpdateAlertStatusCommandValidator"/>.
/// </summary>
public class UpdateAlertStatusCommandValidatorTests
{
    private readonly UpdateAlertStatusCommandValidator _validator = new();

    /// <summary>Returns a fully populated, valid <see cref="UpdateAlertStatusCommand"/> for use as a baseline.</summary>
    private static UpdateAlertStatusCommand ValidCommand() => new()
    {
        AlertId = Guid.NewGuid(),
        Status = "Acknowledged"
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
    /// Verifies that an empty <c>AlertId</c> produces a validation error on the <c>AlertId</c> field.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyAlertId_Fails()
    {
        var cmd = ValidCommand();
        cmd.AlertId = Guid.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.AlertId));
    }

    /// <summary>
    /// Verifies that an empty <c>Status</c> string produces a validation error on the <c>Status</c> field.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyStatus_Fails()
    {
        var cmd = ValidCommand();
        cmd.Status = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Status));
    }

    /// <summary>
    /// Verifies that each valid alert status string passes validation,
    /// including case-insensitive variants.
    /// </summary>
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

    /// <summary>
    /// Verifies that unrecognised status strings fail validation on the <c>Status</c> field.
    /// </summary>
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

    /// <summary>
    /// Verifies that a <c>Status</c> string exceeding the 50-character maximum fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_StatusExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Status = new string('A', 51);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that a null <c>UserId</c> (optional field) does not affect validation.
    /// </summary>
    [Fact]
    public async Task Validate_NullUserId_IsAllowed()
    {
        var cmd = ValidCommand();
        cmd.UserId = null;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }
}
