using Dsptch.Handlers;

namespace Dsptch.WebApi.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    public Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        return Task.FromResult(Guid.NewGuid());     
    }
}