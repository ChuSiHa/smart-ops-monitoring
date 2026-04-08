namespace SmartOpsMonitoring.Domain.Entities;

/// <summary>
/// Base class for all domain entities providing common identity and audit fields.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Gets or sets the unique identifier for this entity.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the UTC timestamp when this entity was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the UTC timestamp when this entity was last updated.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
