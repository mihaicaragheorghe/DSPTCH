using Dsptch.Contracts;
using Dsptch.Decorators;

using Microsoft.Extensions.Caching.Memory;

namespace Dsptch.WebApi.Decorators;

public class CachingDecorator<TQuery, TResult>
    : IDispatcherDecorator<TQuery, TResult> where TQuery : IQuery<TResult>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingDecorator<TQuery, TResult>> _logger;

    public CachingDecorator(IMemoryCache cache, ILogger<CachingDecorator<TQuery, TResult>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResult> Dispatch(TQuery query, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var queryName = typeof(TQuery).Name;
        var cacheKey = $"{typeof(TQuery).Name}:{query.GetHashCode()}";

        if (_cache.TryGetValue(cacheKey, out TResult? result))
        {
            _logger.LogInformation("Cache hit for query: {query}", queryName);

            return result ?? throw new InvalidOperationException("Cache entry is null");
        }

        _logger.LogInformation("Cache miss for query: {query}", queryName);

        result = await next();

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));

        return result;
    }
}