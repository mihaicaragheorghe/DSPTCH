using Dsptch.Handlers;

namespace Dsptch.WebApi.Commands;

public class SampleCommandHandler : IRequestHandler<SampleCommand, string>
{
    public Task<string> Handle(SampleCommand command, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"Hello, {command.Name}! You are {command.Age} years old.");
    }
}