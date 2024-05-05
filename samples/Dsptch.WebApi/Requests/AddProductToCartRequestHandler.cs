using Dsptch.Handlers;

namespace Dsptch.WebApi.Requests;

public class AddProductToCartRequestHandler
    : IRequestHandler<AddProductToCartRequest, bool>
{
    public Task<bool> Handle(
        AddProductToCartRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}