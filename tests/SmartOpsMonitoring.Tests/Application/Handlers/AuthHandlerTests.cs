using FluentAssertions;
using Moq;
using SmartOpsMonitoring.Application.Contracts;
using SmartOpsMonitoring.Application.DTOs.Auth;
using SmartOpsMonitoring.Application.Features.Auth.Commands.Login;
using SmartOpsMonitoring.Application.Features.Auth.Commands.RegisterUser;

namespace SmartOpsMonitoring.Tests.Application.Handlers;

public class AuthHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();

    // --- LoginCommandHandler ---

    [Fact]
    public async Task LoginHandler_ValidCredentials_ReturnsLoginResultDto()
    {
        var expected = new LoginResultDto { Token = "jwt-token", ExpiresAt = DateTime.UtcNow.AddHours(24) };
        _authServiceMock.Setup(s => s.LoginAsync("user@test.com", "Password1!", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new LoginCommandHandler(_authServiceMock.Object);
        var result = await handler.Handle(
            new LoginCommand { Email = "user@test.com", Password = "Password1!" },
            CancellationToken.None);

        result.Should().NotBeNull();
        result.Token.Should().Be("jwt-token");
        _authServiceMock.Verify(s => s.LoginAsync("user@test.com", "Password1!", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginHandler_InvalidCredentials_PropagatesException()
    {
        _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials."));

        var handler = new LoginCommandHandler(_authServiceMock.Object);

        await handler.Invoking(h => h.Handle(
                new LoginCommand { Email = "bad@test.com", Password = "wrong" },
                CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    // --- RegisterUserCommandHandler ---

    [Fact]
    public async Task RegisterHandler_ValidCommand_ReturnsRegisterResultDto()
    {
        var expected = new RegisterResultDto { Message = "User registered successfully." };
        _authServiceMock.Setup(s => s.RegisterAsync("new@test.com", "Password1!", "New User", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new RegisterUserCommandHandler(_authServiceMock.Object);
        var result = await handler.Handle(
            new RegisterUserCommand { Email = "new@test.com", Password = "Password1!", DisplayName = "New User" },
            CancellationToken.None);

        result.Message.Should().Be("User registered successfully.");
        _authServiceMock.Verify(
            s => s.RegisterAsync("new@test.com", "Password1!", "New User", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterHandler_NullDisplayName_DelegatesToAuthService()
    {
        var expected = new RegisterResultDto { Message = "User registered successfully." };
        _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new RegisterUserCommandHandler(_authServiceMock.Object);
        var result = await handler.Handle(
            new RegisterUserCommand { Email = "user@test.com", Password = "Password1!", DisplayName = null },
            CancellationToken.None);

        result.Should().NotBeNull();
        _authServiceMock.Verify(
            s => s.RegisterAsync("user@test.com", "Password1!", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterHandler_FailedRegistration_PropagatesException()
    {
        _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Registration failed: Email already taken."));

        var handler = new RegisterUserCommandHandler(_authServiceMock.Object);

        await handler.Invoking(h => h.Handle(
                new RegisterUserCommand { Email = "dup@test.com", Password = "Password1!" },
                CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Registration failed: Email already taken.");
    }
}
