using System.Net;

namespace SmartOpsMonitoring.Domain.ValueObjects;

/// <summary>
/// Value object representing a validated IP address.
/// </summary>
public record IpAddress
{
    /// <summary>Gets the string value of the IP address.</summary>
    public string Value { get; }

    private IpAddress(string value) => Value = value;

    /// <summary>
    /// Creates an <see cref="IpAddress"/> instance after validating the supplied string.
    /// An empty or null input returns an <see cref="IpAddress"/> with an empty value.
    /// </summary>
    /// <param name="value">The IP address string to validate.</param>
    /// <returns>A valid <see cref="IpAddress"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the supplied value is not a valid IP address.</exception>
    public static IpAddress Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new IpAddress(string.Empty);

        if (!System.Net.IPAddress.TryParse(value, out _))
            throw new ArgumentException($"'{value}' is not a valid IP address.", nameof(value));

        return new IpAddress(value);
    }

    /// <summary>Returns the string representation of this IP address.</summary>
    public override string ToString() => Value;
}
