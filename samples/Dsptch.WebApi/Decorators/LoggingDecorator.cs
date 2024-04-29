using Dsptch.Contracts;
using Dsptch.Decorators;

namespace Dsptch.WebApi.Decorators;

public class LoggingDecorator<TRequest, TResult>(ILogger<LoggingDecorator<TRequest, TResult>> logger)
    : IDispatcherDecorator<TRequest, TResult> where TRequest : IRequest<TResult>
{
    public async Task<TResult> Dispatch(TRequest command, RequestHandlerDelegate<TResult> handler, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TRequest).Name;
        logger.LogInformation("Dispatching command: {command}", commandName);

        try
        {
            var result = await handler();

            logger.LogInformation("Command dispatched: {command}", commandName);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error dispatching command: {command}", commandName);
            throw;
        }
    }
}