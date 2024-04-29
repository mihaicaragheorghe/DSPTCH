using Dsptch.Contracts;

namespace Dsptch.WebApi.Queries;

public record QueryWithCustomDecorator(string Value) : IQuery<string>;