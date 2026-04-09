using FluentAssertions;
using SmartOpsMonitoring.Domain.ValueObjects;

namespace SmartOpsMonitoring.Tests.Domain;

/// <summary>
/// Unit tests for the <see cref="IpAddress"/> value object.
/// </summary>
public class IpAddressTests
{
    /// <summary>
    /// Verifies that a well-formed IPv4 or IPv6 string produces an <see cref="IpAddress"/>
    /// whose <c>Value</c> matches the input.
    /// </summary>
    [Theory]
    [InlineData("192.168.1.1")]
    [InlineData("10.0.0.1")]
    [InlineData("172.16.254.1")]
    [InlineData("255.255.255.255")]
    [InlineData("::1")]
    [InlineData("2001:db8::1")]
    public void Create_ValidIpAddress_ReturnsIpAddress(string ip)
    {
        var result = IpAddress.Create(ip);

        result.Value.Should().Be(ip);
    }

    /// <summary>
    /// Verifies that a null, empty, or whitespace-only input returns an
    /// <see cref="IpAddress"/> with an empty <c>Value</c> rather than throwing.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrWhitespace_ReturnsEmptyIpAddress(string? ip)
    {
        var result = IpAddress.Create(ip);

        result.Value.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that an invalid IP string throws an <see cref="ArgumentException"/>
    /// whose message identifies the offending value.
    /// </summary>
    [Theory]
    [InlineData("999.999.999.999")]
    [InlineData("not-an-ip")]
    [InlineData("256.0.0.1")]
    [InlineData("abc")]
    public void Create_InvalidIpAddress_ThrowsArgumentException(string ip)
    {
        Action act = () => IpAddress.Create(ip);

        act.Should().Throw<ArgumentException>()
            .WithMessage($"'{ip}' is not a valid IP address.*");
    }

    /// <summary>
    /// Verifies that <see cref="IpAddress.ToString"/> returns the underlying IP string.
    /// </summary>
    [Fact]
    public void ToString_ReturnsValue()
    {
        var ip = IpAddress.Create("10.0.0.5");

        ip.ToString().Should().Be("10.0.0.5");
    }

    /// <summary>
    /// Verifies that two <see cref="IpAddress"/> instances created from the same string
    /// are considered equal (record value semantics).
    /// </summary>
    [Fact]
    public void Create_SameIp_EqualValueObjects()
    {
        var ip1 = IpAddress.Create("192.168.0.1");
        var ip2 = IpAddress.Create("192.168.0.1");

        ip1.Should().Be(ip2);
    }

    /// <summary>
    /// Verifies that two <see cref="IpAddress"/> instances created from different strings
    /// are not equal.
    /// </summary>
    [Fact]
    public void Create_DifferentIps_NotEqual()
    {
        var ip1 = IpAddress.Create("192.168.0.1");
        var ip2 = IpAddress.Create("192.168.0.2");

        ip1.Should().NotBe(ip2);
    }
}
