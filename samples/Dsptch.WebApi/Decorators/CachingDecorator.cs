using Dsptch.Contracts;
using Dsptch.Decorators;

using Microsoft.Extensions.Caching.Memory;

namespace Dsptch.WebApi.Decorators;

public class CachingDecorator<TQuery, TResult>(IMemoryCache cache, ILogger<CachingDecorator<TQuery, TResult>> logger)
    : IDispatcherDecorator<TQuery, TResult> where TQuery : IQuery<TResult>
{
    public async Task<TResult> Dispatch(TQuery query, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var queryName = typeof(TQuery).Name;
        var cacheKey = $"{typeof(TQuery).Name}:{query.GetHashCode()}";

        if (cache.TryGetValue(cacheKey, out TResult? result))
        {
            logger.LogInformation("Cache hit for query: {query}", queryName);

            return result ?? throw new InvalidOperationException("Cache entry is null");
        }

        logger.LogInformation("Cache miss for query: {query}", queryName);

        result = await next();

        cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));

        return result;
    }
}