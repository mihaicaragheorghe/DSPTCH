using Dsptch.Contracts;

namespace Dsptch.WebApi.Queries;

public record SampleQuery(string Name) : IQuery<string>;