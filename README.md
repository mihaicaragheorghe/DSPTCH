# DSPTCH

Lightweight library for implementing CQRS and event dispatching in .NET.

## CQRS

Create a request, query or command by implementing the `IRequest`, `IQuery` or `ICommand` interface.

```csharp
public record GetProductByIdQuery(Guid Id)
    : IQuery<Product>;

public record GetProductByNameQuery(string Name)
    : IRequest<List<Product>>;

public record CreateProductCommand(string Name, decimal Price)
    : ICommand<Guid>;
```

`IQuery` and `ICommand` inherit from `IRequest`.

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
app.MapGet("/products/{id}", async (IDispatcher dispatcher, Guid id) =>
{
    Product? product = await dispatcher.Dispatch<GetProductByIdQuery, Product?>(new GetProductByIdQuery(id));

    return product is not null ? Results.Ok(product) : Results.NotFound();
});
```

## Dispatch decorators

Use dispatch decorators to add cross-cutting concerns to the dispatching process.

```csharp
// This decorator will be called before and after every request is dispatched
public class LoggingDecorator<TRequest, TResult>(ILogger<TRequest> logger)
    : IDispatcherDecorator<TRequest, TResult>
        where TRequest : IRequest<TResult>
{
    public async Task<TResult> Dispatch(
        TRequest command,
        RequestHandlerDelegate<TResult> handler,
        CancellationToken cancellationToken = default)
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

// This decorator will be called before and after every query is dispatched
// It won't be called for IRequestHandler or ICommandHandler implementations
public class CachingDecorator<TQuery, TResult>(
    IMemoryCache cache,
    ILogger<CachingDecorator<TQuery, TResult>> logger)
        : IDispatcherDecorator<TQuery, TResult>
            where TQuery : IQuery<TResult>
{
}

// This decorator will only be called before and after every IAuthorizableRequest is dispatched
public class AuthorizationDecorator<TQuery, TResult>
    : IDispatcherDecorator<TQuery, TResult>
        where TQuery : IAuthorizableRequest<TResult>
{
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
});

// Or register dispatchers manually
builder.Services.TryAddTransient(typeof(IDispatcherDecorator<,>), typeof(LoggingDecorator<,>));
builder.Services.TryAddTransient(typeof(IDispatcherDecorator<,>), typeof(CachingDecorator<,>));
```

- The `AddDsptch` method will register the dispatcher and handlers in the specified assemblies.
  - They will be registered as transient services by default, but this can be changed via `opts.Lifetime`.
- The decorators can be registered using the `RegisterDispatcherDecorator` method.

## License

This project is licensed under the MIT License - see the [LICENSE file](https://github.com/mihaicaragheorghe/DSPTCH/blob/main/LICENSE) for details.
