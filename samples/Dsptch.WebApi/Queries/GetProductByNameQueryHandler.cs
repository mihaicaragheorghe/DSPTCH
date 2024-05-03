using Dsptch.Handlers;
using Dsptch.WebApi.Models;

namespace Dsptch.WebApi.Queries;

public class GetProductByNameQueryHandler : IRequestHandler<GetProductByNameQuery, List<Product>>
{
    public Task<List<Product>> Handle(GetProductByNameQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<Product>
        {
            new(Guid.NewGuid(), query.Name, 100.00m)
        });
    }
}