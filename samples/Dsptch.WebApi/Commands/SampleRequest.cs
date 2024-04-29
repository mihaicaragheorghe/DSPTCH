using Dsptch.Contracts;

namespace Dsptch.WebApi.Commands;

public record SampleRequest(int Id, string Name) : IRequest<string>;