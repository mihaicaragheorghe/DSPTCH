using Dsptch.Contracts;

namespace Dsptch.WebApi.Commands;

public record CreateProductCommand(string Name, decimal Price)
    : ICommand<Guid>;