using FluentAssertions;
using SmartOpsMonitoring.Domain.ValueObjects;

namespace SmartOpsMonitoring.Tests.Domain;

public class IpAddressTests
{
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrWhitespace_ReturnsEmptyIpAddress(string? ip)
    {
        var result = IpAddress.Create(ip);

        result.Value.Should().BeEmpty();
    }

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

    [Fact]
    public void ToString_ReturnsValue()
    {
        var ip = IpAddress.Create("10.0.0.5");

        ip.ToString().Should().Be("10.0.0.5");
    }

    [Fact]
    public void Create_SameIp_EqualValueObjects()
    {
        var ip1 = IpAddress.Create("192.168.0.1");
        var ip2 = IpAddress.Create("192.168.0.1");

        ip1.Should().Be(ip2);
    }

    [Fact]
    public void Create_DifferentIps_NotEqual()
    {
        var ip1 = IpAddress.Create("192.168.0.1");
        var ip2 = IpAddress.Create("192.168.0.2");

        ip1.Should().NotBe(ip2);
    }
}
