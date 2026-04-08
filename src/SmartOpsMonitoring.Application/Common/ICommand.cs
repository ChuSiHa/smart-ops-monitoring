using MediatR;

namespace SmartOpsMonitoring.Application.Common;

/// <summary>
/// Marker interface for commands that return a result.
/// Inherits <see cref="IRequest{TResult}"/> so MediatR can dispatch it.
/// </summary>
/// <typeparam name="TResult">The type returned after handling the command.</typeparam>
public interface ICommand<TResult> : IRequest<TResult>
{
}
