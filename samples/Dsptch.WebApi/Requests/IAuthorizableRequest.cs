using Dsptch.Contracts;

namespace Dsptch.WebApi.Requests;

public interface IAuthorizableRequest<TResult> : IRequest<TResult>
{
    Guid UserId { get; }
}