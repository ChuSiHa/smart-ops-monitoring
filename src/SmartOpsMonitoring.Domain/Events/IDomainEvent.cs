using MediatR;

namespace SmartOpsMonitoring.Domain.Events;

/// <summary>
/// Marker interface for all domain events. Inherits <see cref="INotification"/> so events
/// can be dispatched via MediatR's publish pipeline.
/// </summary>
public interface IDomainEvent : INotification
{
}
