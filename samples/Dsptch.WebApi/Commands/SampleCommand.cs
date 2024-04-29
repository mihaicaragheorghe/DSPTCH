using Dsptch.Contracts;

namespace Dsptch.WebApi.Commands;

public record SampleCommand(int Age, string Name)
    : ICommand<string>;
