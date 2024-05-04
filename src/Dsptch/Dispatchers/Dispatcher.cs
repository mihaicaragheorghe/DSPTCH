using Dsptch.Contracts;
using Dsptch.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Dsptch.Decorators;

/// <inheritdoc/>
public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public Task<TResult> Dispatch<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResult>
    {
        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResult>>();

        Task<TResult> Handle() => handler.Handle(request, cancellationToken);

        return _serviceProvider
            .GetServices<IDispatcherDecorator<TRequest, TResult>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<TResult>)Handle, (next, dispatcher) => () => dispatcher.Dispatch(request, next, cancellationToken))();
    }
}