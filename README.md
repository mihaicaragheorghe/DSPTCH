# DSPTCH

Lightweight library for implementing CQRS and event dispatching in .NET.

Efficiently handle requests and commands, decoupling the sender from the receiver, and facilitating cleaner, more maintainable code.

## Dependecy Injection

Use microsoft DI to register the dispatcher and handlers.

```csharp
services.AddDsptch(opts =>
{
    opts.RegisterServicesFromAssemblyContaining<Program>();
});
```

## CQRS

Create a request, query or command by implementing the `IRequest`, `IQuery` or `ICommand` interface.

```csharp
public record GetProductByIdQuery(Guid Id) : IQuery<Product>;

public record CreateProductCommand(string Name) : ICommand<Guid>;
```

IQuery and ICommand inherit from IRequest.

## Handlers

Create a handler for each request, query or command by implementing the `IRequestHandler` interface.

```csharp
public class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductByIdQuery, Product?>
{
    public Task<Product?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        return productsRepository.GetById(query.Id);
    }
}
```

## Dispatch

Dispatch the request, query or command using the `IDispather` interface.

```csharp
public class ProductsController(IDispatcher dispatcher)
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        Product? product = await dispatcher.Dispatch(new GetProductByIdQuery(id));
        return product is null ? NotFound() : Ok(product);
    }
}
```

## Dispatch decorators

Use dispatch decorators to add cross-cutting concerns to the dispatching process.

```csharp
public class LoggingDecorator<TRequest, TResult>(ILogger<LoggingDecorator<TRequest, TResult>> logger)
    : IDispatcherDecorator<TRequest, TResult> where TRequest : IRequest<TResult>
{
    public async Task<TResult> Dispatch(TRequest command, RequestHandlerDelegate<TResult> handler, CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Dispatching request: {req}", requestName);

        try
        {
            var result = await handler();

            logger.LogInformation("Request dispatched: {req}", requestName);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error dispatching request: {req}", requestName);
            throw;
        }
    }
}
```

## Dependency Injection

Use Microsoft DI to register the dispatcher and handlers.

```csharp
builder.Services.AddDsptch(opts =>
{
    opts.RegisterServicesFromAssemblyContaining<Program>();

    // Register dispatchers using DsptchConfiguration
    opts.RegisterDispatcherDecorator(typeof(LoggingDecorator<,>));
    opts.RegisterDispatcherDecorator(typeof(CachingDecorator<,>));
    opts.RegisterDispatcherDecorator(typeof(CustomQueryDecorator<,>));
});

// Or register dispatchers manually
builder.Services.TryAddTransient(typeof(IDispatcherDecorator<,>), typeof(LoggingDecorator<,>));
builder.Services.TryAddTransient(typeof(IDispatcherDecorator<,>), typeof(CachingDecorator<,>));
builder.Services.TryAddTransient(typeof(IDispatcherDecorator<,>), typeof(CustomQueryDecorator<,>));
```

The `RegisterServicesFromAssemblyContaining` method will register all handlers in the assembly containing the specified type.

## License

This project is licensed under the MIT License - see the [LICENSE file](https://github.com/mihaicaragheorghe/DSPTCH/blob/main/LICENSE) for details.
