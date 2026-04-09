using FluentAssertions;
using Moq;
using SmartOpsMonitoring.Application.Features.ServiceNodes.Commands.CreateServiceNode;
using SmartOpsMonitoring.Application.Features.ServiceNodes.Queries.GetServiceNodesByHost;
using SmartOpsMonitoring.Application.Mappings;
using SmartOpsMonitoring.Domain.Entities;
using SmartOpsMonitoring.Domain.Enums;
using SmartOpsMonitoring.Domain.Repositories;

namespace SmartOpsMonitoring.Tests.Application.Handlers;

/// <summary>
/// Unit tests for service-node-related CQRS handlers:
/// <see cref="CreateServiceNodeCommandHandler"/> and <see cref="GetServiceNodesByHostQueryHandler"/>.
/// </summary>
public class ServiceNodeHandlerTests
{
    private readonly Mock<IServiceNodeRepository> _serviceNodeRepositoryMock = new();

    /// <summary>Registers Mapster mappings required by the handlers under test.</summary>
    public ServiceNodeHandlerTests()
    {
        MappingConfig.RegisterMappings();
    }

    // --- CreateServiceNodeCommandHandler ---

    /// <summary>
    /// Verifies that handling a valid create command persists the service node to the repository
    /// and returns a correctly mapped <see cref="Application.DTOs.ServiceNodeDto"/>.
    /// </summary>
    [Fact]
    public async Task CreateServiceNodeHandler_ValidCommand_AddsAndReturnsDto()
    {
        var handler = new CreateServiceNodeCommandHandler(_serviceNodeRepositoryMock.Object);
        var hostId = Guid.NewGuid();
        var command = new CreateServiceNodeCommand
        {
            Name = "nginx",
            Type = "web",
            HostId = hostId,
            Port = 443
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("nginx");
        result.Type.Should().Be("web");
        result.HostId.Should().Be(hostId);
        result.Port.Should().Be(443);
        result.Status.Should().Be(ServiceNodeStatus.Unknown);

        _serviceNodeRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<ServiceNode>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that when no port is provided the returned DTO has a null <c>Port</c>.
    /// </summary>
    [Fact]
    public async Task CreateServiceNodeHandler_WithoutPort_PortIsNull()
    {
        var handler = new CreateServiceNodeCommandHandler(_serviceNodeRepositoryMock.Object);
        var command = new CreateServiceNodeCommand
        {
            Name = "worker",
            Type = "background",
            HostId = Guid.NewGuid(),
            Port = null
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Port.Should().BeNull();
    }

    // --- GetServiceNodesByHostQueryHandler ---

    /// <summary>
    /// Verifies that the handler returns all service nodes for the specified host, mapped to DTOs.
    /// </summary>
    [Fact]
    public async Task GetServiceNodesByHostHandler_ExistingHost_ReturnsNodes()
    {
        var hostId = Guid.NewGuid();
        var nodes = new List<ServiceNode>
        {
            new() { Id = Guid.NewGuid(), Name = "api", Type = "web", HostId = hostId, Port = 5000 },
            new() { Id = Guid.NewGuid(), Name = "db", Type = "database", HostId = hostId, Port = 5432 }
        };
        _serviceNodeRepositoryMock.Setup(r => r.GetByHostIdAsync(hostId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(nodes);

        var handler = new GetServiceNodesByHostQueryHandler(_serviceNodeRepositoryMock.Object);
        var result = await handler.Handle(new GetServiceNodesByHostQuery { HostId = hostId }, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(n => n.Name).Should().Contain(new[] { "api", "db" });
    }

    /// <summary>
    /// Verifies that when a host has no service nodes, the handler returns an empty collection.
    /// </summary>
    [Fact]
    public async Task GetServiceNodesByHostHandler_NoNodes_ReturnsEmpty()
    {
        var hostId = Guid.NewGuid();
        _serviceNodeRepositoryMock.Setup(r => r.GetByHostIdAsync(hostId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ServiceNode>());

        var handler = new GetServiceNodesByHostQueryHandler(_serviceNodeRepositoryMock.Object);
        var result = await handler.Handle(new GetServiceNodesByHostQuery { HostId = hostId }, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
