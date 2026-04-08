using MediatR;

namespace SmartOpsMonitoring.Application.Common;

/// <summary>
/// Marker interface for query handlers.
/// Inherits <see cref="IRequestHandler{TRequest,TResponse}"/> so MediatR can resolve the handler.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResult">The result type returned by the handler.</typeparam>
public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
}
