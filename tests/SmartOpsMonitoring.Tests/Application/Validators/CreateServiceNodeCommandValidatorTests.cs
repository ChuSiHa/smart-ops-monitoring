using FluentAssertions;
using SmartOpsMonitoring.Application.Features.ServiceNodes.Commands.CreateServiceNode;

namespace SmartOpsMonitoring.Tests.Application.Validators;

public class CreateServiceNodeCommandValidatorTests
{
    private readonly CreateServiceNodeCommandValidator _validator = new();

    private static CreateServiceNodeCommand ValidCommand() => new()
    {
        Name = "nginx",
        Type = "web",
        HostId = Guid.NewGuid(),
        Port = 80
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
        cmd.Name = new string('N', 201);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Name));
    }

    [Fact]
    public async Task Validate_EmptyType_Fails()
    {
        var cmd = ValidCommand();
        cmd.Type = string.Empty;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Type));
    }

    [Fact]
    public async Task Validate_TypeExceedsMaxLength_Fails()
    {
        var cmd = ValidCommand();
        cmd.Type = new string('T', 101);
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(cmd.Type));
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

    [Fact]
    public async Task Validate_NullPort_Passes()
    {
        var cmd = ValidCommand();
        cmd.Port = null;
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }
}
