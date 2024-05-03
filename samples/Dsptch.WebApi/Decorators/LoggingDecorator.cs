using Dsptch.Contracts;
using Dsptch.Decorators;

namespace Dsptch.WebApi.Decorators;

public class LoggingDecorator<TRequest, TResult>(ILogger<LoggingDecorator<TRequest, TResult>> logger)
    : IDispatcherDecorator<TRequest, TResult> where TRequest : IRequest<TResult>
{
    public async Task<TResult> Dispatch(TRequest command, RequestHandlerDelegate<TResult> handler, CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Dispatching request: {req}", requestName);

        try
        {
            var result = await handler();

            logger.LogInformation("Request dispatched: {req}", requestName);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error dispatching request: {req}", requestName);
            throw;
        }
    }
}