using Dsptch.Contracts;
using Dsptch.Decorators;

namespace Dsptch.WebApi.Decorators;

public class LoggingDecorator<TRequest, TResult>
    : IDispatcherDecorator<TRequest, TResult>
        where TRequest : IRequest<TResult>
{
    private readonly ILogger<TRequest> _logger;

    public LoggingDecorator(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> Dispatch(
        TRequest command,
        RequestHandlerDelegate<TResult> handler,
        CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Dispatching request: {req}", requestName);

        try
        {
            var result = await handler();

            _logger.LogInformation("Request dispatched: {req}", requestName);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching request: {req}", requestName);
            throw;
        }
    }
}