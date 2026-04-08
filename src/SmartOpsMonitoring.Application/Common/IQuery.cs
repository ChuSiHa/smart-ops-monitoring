using MediatR;

namespace SmartOpsMonitoring.Application.Common;

/// <summary>
/// Marker interface for queries that return a result.
/// Inherits <see cref="IRequest{TResult}"/> so MediatR can dispatch it.
/// </summary>
/// <typeparam name="TResult">The type returned after handling the query.</typeparam>
public interface IQuery<TResult> : IRequest<TResult>
{
}
