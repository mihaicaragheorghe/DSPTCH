using Dsptch.Contracts;
using Dsptch.WebApi.Models;

namespace Dsptch.WebApi.Queries;

public record GetProductByNameQuery(string Name)
    : IRequest<List<Product>>;