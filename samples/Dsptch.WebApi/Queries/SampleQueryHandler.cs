using Dsptch.Handlers;

namespace Dsptch.WebApi.Queries;

public class SampleQueryHandler(ILogger<SampleQueryHandler> logger) : IRequestHandler<SampleQuery, string>
{
    public Task<string> Handle(SampleQuery query, CancellationToken cancellationToken = default)
    {
        var result = $"Hello, {query.Name}!";

        logger.LogInformation(result);

        return Task.FromResult(result);
    }
}