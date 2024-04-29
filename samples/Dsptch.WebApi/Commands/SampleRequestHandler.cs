using Dsptch.Handlers;

namespace Dsptch.WebApi.Commands;

public class SampleRequestHandler : IRequestHandler<SampleRequest, string>
{
    public Task<string> Handle(SampleRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Hello {request.Name} with Id {request.Id}");
    }
}