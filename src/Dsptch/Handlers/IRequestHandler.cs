using Dsptch.Contracts;

namespace Dsptch.Handlers;

/// <summary>
/// Defines a handler for a request
/// </summary>
/// <typeparam name="TRequest">The type of request to handle</typeparam>
/// <typeparam name="TResult">The type of result from the request</typeparam>
public interface IRequestHandler<in TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    /// <summary>
    /// Handles the request
    /// </summary>
    /// <param name="request">The request to handle</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result of the request</returns>
    Task<TResult> Handle(TRequest request, CancellationToken cancellationToken);
}