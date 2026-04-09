using FluentAssertions;
using Moq;
using SmartOpsMonitoring.Application.Features.Hosts.Commands.CreateHost;
using SmartOpsMonitoring.Application.Features.Hosts.Queries.GetHostById;
using SmartOpsMonitoring.Application.Features.Hosts.Queries.GetHosts;
using SmartOpsMonitoring.Application.Mappings;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Domain.Repositories;
using Host = SmartOpsMonitoring.Domain.Entities.Host;

namespace SmartOpsMonitoring.Tests.Application.Handlers;

/// <summary>
/// Unit tests for host-related CQRS handlers:
/// <see cref="CreateHostCommandHandler"/>, <see cref="GetHostsQueryHandler"/>,
/// and <see cref="GetHostByIdQueryHandler"/>.
/// </summary>
public class HostHandlerTests
{
    private readonly Mock<IHostRepository> _hostRepositoryMock = new();

    /// <summary>Registers Mapster mappings required by the handlers under test.</summary>
    public HostHandlerTests()
    {
        MappingConfig.RegisterMappings();
    }

    // --- CreateHostCommandHandler ---

    /// <summary>
    /// Verifies that handling a valid create command persists the host to the repository
    /// and returns a correctly mapped <see cref="Application.DTOs.HostDto"/>.
    /// </summary>
    [Fact]
    public async Task CreateHostHandler_ValidCommand_AddsAndReturnsHostDto()
    {
        var handler = new CreateHostCommandHandler(_hostRepositoryMock.Object);
        var command = new CreateHostCommand
        {
            Name = "db-server-01",
            OsType = "Linux",
            IpAddress = "10.0.0.5",
            Tags = new List<string> { "database", "production" }
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("db-server-01");
        result.OsType.Should().Be("Linux");
        result.IpAddress.Should().Be("10.0.0.5");
        result.Status.Should().Be(HostStatus.Unknown);

        _hostRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Host>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that a host can be created without providing an IP address.
    /// </summary>
    [Fact]
    public async Task CreateHostHandler_WithoutIpAddress_ReturnsHostDto()
    {
        var handler = new CreateHostCommandHandler(_hostRepositoryMock.Object);
        var command = new CreateHostCommand { Name = "host-02", OsType = "Windows" };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("host-02");
    }

    // --- GetHostsQueryHandler ---

    /// <summary>
    /// Verifies that the handler returns all hosts from the repository mapped to DTOs.
    /// </summary>
    [Fact]
    public async Task GetHostsHandler_ReturnsMappedHostDtos()
    {
        var hosts = new List<Host>
        {
            new() { Id = Guid.NewGuid(), Name = "host-a", OsType = "Linux" },
            new() { Id = Guid.NewGuid(), Name = "host-b", OsType = "Windows" }
        };
        _hostRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(hosts);

        var handler = new GetHostsQueryHandler(_hostRepositoryMock.Object);
        var result = await handler.Handle(new GetHostsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(h => h.Name).Should().Contain(new[] { "host-a", "host-b" });
    }

    /// <summary>
    /// Verifies that when the repository contains no hosts, the handler returns an empty collection.
    /// </summary>
    [Fact]
    public async Task GetHostsHandler_EmptyRepository_ReturnsEmpty()
    {
        _hostRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Host>());

        var handler = new GetHostsQueryHandler(_hostRepositoryMock.Object);
        var result = await handler.Handle(new GetHostsQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    // --- GetHostByIdQueryHandler ---

    /// <summary>
    /// Verifies that when a host with the requested identifier exists, it is returned
    /// as a correctly mapped DTO.
    /// </summary>
    [Fact]
    public async Task GetHostByIdHandler_ExistingId_ReturnsMappedDto()
    {
        var hostId = Guid.NewGuid();
        var host = new Host { Id = hostId, Name = "server-x", OsType = "Linux" };
        _hostRepositoryMock.Setup(r => r.GetByIdAsync(hostId, It.IsAny<CancellationToken>())).ReturnsAsync(host);

        var handler = new GetHostByIdQueryHandler(_hostRepositoryMock.Object);
        var result = await handler.Handle(new GetHostByIdQuery { Id = hostId }, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(hostId);
        result.Name.Should().Be("server-x");
    }

    /// <summary>
    /// Verifies that when no host matches the requested identifier, the handler returns <c>null</c>.
    /// </summary>
    [Fact]
    public async Task GetHostByIdHandler_NonExistingId_ReturnsNull()
    {
        _hostRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Host?)null);

        var handler = new GetHostByIdQueryHandler(_hostRepositoryMock.Object);
        var result = await handler.Handle(new GetHostByIdQuery { Id = Guid.NewGuid() }, CancellationToken.None);

        result.Should().BeNull();
    }
}
