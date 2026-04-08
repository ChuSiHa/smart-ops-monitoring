using MediatR;

namespace SmartOpsMonitoring.Application.Common;

/// <summary>
/// Marker interface for command handlers.
/// Inherits <see cref="IRequestHandler{TRequest,TResponse}"/> so MediatR can resolve the handler.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResult">The result type returned by the handler.</typeparam>
public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
}
