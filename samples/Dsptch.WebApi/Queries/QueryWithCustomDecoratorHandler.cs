using Dsptch.Handlers;

namespace Dsptch.WebApi.Queries;

public class QueryWithCustomDecoratorHandler : IRequestHandler<QueryWithCustomDecorator, string>
{
    public Task<string> Handle(QueryWithCustomDecorator query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(query.Value);
    }
}