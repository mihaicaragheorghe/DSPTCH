using Dsptch.Handlers;
using Dsptch.WebApi.Models;

namespace Dsptch.WebApi.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
{
    public Task<Product> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Product(query.Id, "Product Name", 100.00m));
    }
}