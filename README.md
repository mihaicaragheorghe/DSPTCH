# DSPTCH

Lightweight library for implementing CQRS and event dispatching in .NET.

## Installation

Install the package from [NuGet](https://www.nuget.org/packages/Dsptch).

```bash
dotnet add package Dsptch
```

## Setup

Use Microsoft DI to register the dispatcher and handlers.

```csharp
builder.Services.AddDsptch(opts =>
{
    opts.RegisterServicesFromAssemblyContaining<Program>();

    // Registering dispatchers using DsptchConfiguration
    opts.RegisterDispatcherDecorator(typeof(LoggingDecorator<,>));
    opts.RegisterDispatcherDecorator(typeof(CachingDecorator<,>));
    opts.RegisterDispatcherDecorator(typeof(AuthorizationDecorator<,>));
});

// Or registering dispatchers manually
builder.Services.TryAddTransient(typeof(IDispatcherDecorator<,>), typeof(LoggingDecorator<,>));
```

The `AddDsptch` method will register the dispatcher and handlers in the specified assemblies. They will be registered as transient services by default, but this can be changed via `opts.Lifetime`.

The decorators can be registered using the `RegisterDispatcherDecorator` method, and their lifetime can be set via the method parameter.

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
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
{
    public Task<Product> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Product(query.Id, "Product Name", 100.00m));
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

## Dispatcher decorators

Use dispatch decorators to add cross-cutting concerns to the dispatching process.

```csharp
/// <summary>
/// This decorator will log the request before and after it is dispatched.
/// It will be called for all requests (IRequest), including commands and queries.
/// </summary>
public class LoggingDecorator<TRequest, TResult> : IDispatcherDecorator<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    private readonly ILogger<TRequest> _logger;

    public LoggingDecorator(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> Dispatch(
        TRequest request,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Dispatching request: {req}", requestName);

        try
        {
            var result = await next();

            _logger.LogInformation("Request dispatched: {req}", requestName);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching request: {req}", requestName);
            throw;
        }
    }
}

/// <summary>
/// This decorator will only be called only for requests that implement the IQuery interface.
/// It won't be called for commands (ICommand) or requests (IRequest).
/// </summary>
public class CachingDecorator<TQuery, TResult> : IDispatcherDecorator<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
}

/// <summary>
/// This decorator will only be applied to requests that implement the IAuthorizableRequest interface.
/// </summary>
public class AuthorizationDecorator<TRequest, TResult> : IDispatcherDecorator<TRequest, TResult>
    where TRequest : IAuthorizableRequest<TResult>
{
}
```

## License

This project is licensed under the MIT License - see the [LICENSE file](https://github.com/mihaicaragheorghe/DSPTCH/blob/main/LICENSE) for details.
