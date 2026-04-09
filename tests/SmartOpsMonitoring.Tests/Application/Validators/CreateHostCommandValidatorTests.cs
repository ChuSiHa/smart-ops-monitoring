using FluentAssertions;
using SmartOpsMonitoring.Application.Features.Hosts.Commands.CreateHost;

namespace SmartOpsMonitoring.Tests.Application.Validators;

/// <summary>
/// Unit tests for <see cref="CreateHostCommandValidator"/>.
/// </summary>
public class CreateHostCommandValidatorTests
{
    private readonly CreateHostCommandValidator _validator = new();

    /// <summary>Returns a fully populated, valid <see cref="CreateHostCommand"/> for use as a baseline.</summary>
    private static CreateHostCommand ValidCommand() => new()
    {
        Name = "web-server-01",
        OsType = "Linux",
        IpAddress = "192.168.1.10"
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
        cmd.Name = new string('A', 201);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Name));
    }

    /// <summary>
    /// Verifies that an empty <c>OsType</c> produces a validation error on the <c>OsType</c> field.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyOsType_Fails()
    {
        var cmd = ValidCommand();
        cmd.OsType = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.OsType));
    }

    /// <summary>
    /// Verifies that an <c>OsType</c> exceeding the 100-character maximum fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_OsTypeExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.OsType = new string('O', 101);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.OsType));
    }

    /// <summary>
    /// Verifies that a valid IPv4 or IPv6 address string passes validation.
    /// </summary>
    [Theory]
    [InlineData("192.168.1.1")]
    [InlineData("10.0.0.1")]
    [InlineData("::1")]
    public async Task Validate_ValidIpAddress_Passes(string ip)
    {
        var cmd = ValidCommand();
        cmd.IpAddress = ip;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that a malformed IP address string fails validation on the <c>IpAddress</c> field.
    /// </summary>
    [Theory]
    [InlineData("999.999.999.999")]
    [InlineData("not-an-ip")]
    public async Task Validate_InvalidIpAddress_Fails(string ip)
    {
        var cmd = ValidCommand();
        cmd.IpAddress = ip;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.IpAddress));
    }

    /// <summary>
    /// Verifies that omitting the optional <c>IpAddress</c> (empty string) does not affect validation.
    /// </summary>
    [Fact]
    public async Task Validate_EmptyIpAddress_Passes()
    {
        var cmd = ValidCommand();
        cmd.IpAddress = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that an <c>IpAddress</c> exceeding the 45-character maximum fails validation.
    /// </summary>
    [Fact]
    public async Task Validate_IpAddressExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.IpAddress = new string('1', 46);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.IpAddress));
    }
}
