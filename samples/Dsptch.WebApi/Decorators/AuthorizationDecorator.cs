using Dsptch.Decorators;
using Dsptch.WebApi.Requests;

namespace Dsptch.WebApi.Decorators;

/// <summary>
/// This decorator will only be applied to requests that implement the IAuthorizableRequest interface.
/// </summary>
public class AuthorizationDecorator<TRequest, TResult>
    : IDispatcherDecorator<TRequest, TResult>
        where TRequest : IAuthorizableRequest<TResult>
{
    private readonly ILogger<AuthorizationDecorator<TRequest, TResult>> _logger;

    public AuthorizationDecorator(ILogger<AuthorizationDecorator<TRequest, TResult>> logger)
    {
        _logger = logger;
    }

    public Task<TResult> Dispatch(
        TRequest request,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken = default)
    {
        if (request.UserId == Guid.Empty)
        {
            _logger.LogError("User ID is missing");

            throw new InvalidOperationException("User ID is missing");
        }

        _logger.LogInformation("User ID: {UserId}", request.UserId);

        return next();
    }
}