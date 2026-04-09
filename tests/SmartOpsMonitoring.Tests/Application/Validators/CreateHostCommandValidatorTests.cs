using FluentAssertions;
using SmartOpsMonitoring.Application.Features.Hosts.Commands.CreateHost;

namespace SmartOpsMonitoring.Tests.Application.Validators;

public class CreateHostCommandValidatorTests
{
    private readonly CreateHostCommandValidator _validator = new();

    private static CreateHostCommand ValidCommand() => new()
    {
        Name = "web-server-01",
        OsType = "Linux",
        IpAddress = "192.168.1.10"
    };

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        var result = await _validator.ValidateAsync(ValidCommand());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyName_Fails()
    {
        var cmd = ValidCommand();
        cmd.Name = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Name));
    }

    [Fact]
    public async Task Validate_NameExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Name = new string('A', 201);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Name));
    }

    [Fact]
    public async Task Validate_EmptyOsType_Fails()
    {
        var cmd = ValidCommand();
        cmd.OsType = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.OsType));
    }

    [Fact]
    public async Task Validate_OsTypeExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.OsType = new string('O', 101);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.OsType));
    }

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

    [Fact]
    public async Task Validate_EmptyIpAddress_Passes()
    {
        var cmd = ValidCommand();
        cmd.IpAddress = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

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
