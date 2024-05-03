using Dsptch.Contracts;
using Dsptch.WebApi.Models;

namespace Dsptch.WebApi.Queries;

public record GetProductByIdQuery(Guid Id)
    : IQuery<Product>;