namespace Dsptch.WebApi.Requests;

public record AddProductToCartRequest(Guid ProductId, Guid UserId)
    : IAuthorizableRequest<bool>;