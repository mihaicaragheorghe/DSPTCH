using Dsptch.Contracts;

namespace Dsptch.Decorators;

/// <summary>
/// Defines a delegate for a request handler
/// </summary>
/// <typeparam name="TRequest">The type of request to handle</typeparam>
public delegate Task<TRequest> RequestHandlerDelegate<TRequest>();

/// <summary>
/// Defines a dispatcher decorator for a request
/// </summary>
/// <typeparam name="TRequest">The type of request to dispatch</typeparam>
/// <typeparam name="TResult">The type of result from the request</typeparam>
public interface IDispatcherDecorator<in TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    /// <summary>
    /// Dispatches the request to the appropriate handler
    /// </summary>
    /// <param name="request">The request to dispatch</param>
    /// <param name="next">The next handler in the pipeline</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the command</returns>
    Task<TResult> Dispatch(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default);
}