using FluentAssertions;
using SmartOpsMonitoring.Application.Features.Alerts.Commands.CreateAlert;

namespace SmartOpsMonitoring.Tests.Application.Validators;

/// <summary>
/// Unit tests for <see cref="CreateAlertCommandValidator"/>.
/// </summary>
public class CreateAlertCommandValidatorTests
{
    private readonly CreateAlertCommandValidator _validator = new();

    /// <summary>Returns a fully populated, valid <see cref="CreateAlertCommand"/> for use as a baseline.</summary>
    private static CreateAlertCommand ValidCommand() => new()
    {
        HostId = Guid.NewGuid(),
        Title = "High CPU",
        Message = "CPU usage exceeded 90%",
        Severity = "Warning"
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
    /// Verifies that an empty <c>Title</c> produces a validation error on the <c>Title</c> field.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyTitle_Fails()
    {
        var cmd = ValidCommand();
        cmd.Title = string.Empty;

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Title));
    }

    /// <summary>
    /// Verifies that a <c>Title</c> exceeding the 200-character maximum fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_TitleExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Title = new string('A', 201);

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Title));
    }

    /// <summary>
    /// Verifies that an empty <c>Message</c> produces a validation error on the <c>Message</c> field.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyMessage_Fails()
    {
        var cmd = ValidCommand();
        cmd.Message = string.Empty;

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Message));
    }

    /// <summary>
    /// Verifies that a <c>Message</c> exceeding the 2000-character maximum fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_MessageExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Message = new string('M', 2001);

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Message));
    }

    /// <summary>
    /// Verifies that each valid severity string ("Info", "Warning", "Critical") passes validation,
    /// including case-insensitive variants.
    /// </summary>
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

    /// <summary>
    /// Verifies that unrecognised or empty severity strings fail validation on the <c>Severity</c> field.
    /// </summary>
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

    /// <summary>
    /// Verifies that omitting the optional <c>ServiceNodeId</c> (null) does not affect validation.
    /// </summary>
    [Fact]
    public async Task Validate_OptionalServiceNodeId_IsAllowedNull()
    {
        var cmd = ValidCommand();
        cmd.ServiceNodeId = null;

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that providing a valid <c>ServiceNodeId</c> passes validation.
    /// </summary>
    [Fact]
    public async Task Validate_WithServiceNodeId_Passes()
    {
        var cmd = ValidCommand();
        cmd.ServiceNodeId = Guid.NewGuid();

        var result = await _validator.ValidateAsync(cmd);

        result.IsValid.Should().BeTrue();
    }
}
