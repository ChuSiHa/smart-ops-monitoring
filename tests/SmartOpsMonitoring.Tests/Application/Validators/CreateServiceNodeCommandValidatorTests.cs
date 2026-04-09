using FluentAssertions;
using SmartOpsMonitoring.Application.Features.ServiceNodes.Commands.CreateServiceNode;

namespace SmartOpsMonitoring.Tests.Application.Validators;

/// <summary>
/// Unit tests for <see cref="CreateServiceNodeCommandValidator"/>.
/// </summary>
public class CreateServiceNodeCommandValidatorTests
{
    private readonly CreateServiceNodeCommandValidator _validator = new();

    /// <summary>Returns a fully populated, valid <see cref="CreateServiceNodeCommand"/> for use as a baseline.</summary>
    private static CreateServiceNodeCommand ValidCommand() => new()
    {
        Name = "nginx",
        Type = "web",
        HostId = Guid.NewGuid(),
        Port = 80
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
    /// Verifies that an empty <c>Name</c> produces a validation error on the <c>Name</c> field.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyName_Fails()
    {
        var cmd = ValidCommand();
        cmd.Name = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Name));
    }

    /// <summary>
    /// Verifies that a <c>Name</c> exceeding the 200-character maximum fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_NameExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Name = new string('N', 201);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Name));
    }

    /// <summary>
    /// Verifies that an empty <c>Type</c> produces a validation error on the <c>Type</c> field.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyType_Fails()
    {
        var cmd = ValidCommand();
        cmd.Type = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Type));
    }

    /// <summary>
    /// Verifies that a <c>Type</c> exceeding the 100-character maximum fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_TypeExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Type = new string('T', 101);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Type));
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
    /// Verifies that port numbers within the valid range (1–65535) pass validation.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(80)]
    [InlineData(443)]
    [InlineData(65535)]
    public async Task Validate_ValidPort_Passes(int port)
    {
        var cmd = ValidCommand();
        cmd.Port = port;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that port numbers outside the valid range (0, negative, or above 65535) fail validation.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    [InlineData(99999)]
    public async Task Validate_InvalidPort_Fails(int port)
    {
        var cmd = ValidCommand();
        cmd.Port = port;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Port));
    }

    /// <summary>
    /// Verifies that omitting the optional <c>Port</c> (null) does not affect validation.
    /// </summary>
    [Fact]
    public async Task Validate_NullPort_Passes()
    {
        var cmd = ValidCommand();
        cmd.Port = null;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }
}
