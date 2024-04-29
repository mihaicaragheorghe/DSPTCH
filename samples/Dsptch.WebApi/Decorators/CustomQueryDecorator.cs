using Dsptch.Contracts;
using Dsptch.Decorators;
using Dsptch.WebApi.Queries;

namespace Dsptch.WebApi.Decorators;

public class CustomQueryDecorator<TQuery, TResult>(ILogger<CustomQueryDecorator<TQuery, TResult>> logger)
    : IDispatcherDecorator<TQuery, TResult> where TQuery : QueryWithCustomDecorator, IQuery<TResult>
{
    public Task<TResult> Dispatch(TQuery query, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("CustomQueryDecorator: Before dispatching the query");

        return next();
    }
}