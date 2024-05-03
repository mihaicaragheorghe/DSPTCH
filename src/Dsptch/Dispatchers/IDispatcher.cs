using Dsptch.Contracts;

namespace Dsptch.Decorators;

/// <summary>
/// Defines a dispatcher for a request
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Dispatches the request to the appropriate handler
    /// </summary>
    /// <param name="request">The request to dispatch</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <typeparam name="TRequest">The type of request to dispatch</typeparam>
    /// <typeparam name="TResult">The type of result from the request</typeparam>
    /// <returns>The result of the command</returns>
    Task<TResult> Dispatch<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResult>;

    /// <inheritdoc cref="Dispatch{TRequest,TResult}(TRequest,CancellationToken)"/>
    Task<TResult> Dispatch<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);
}
