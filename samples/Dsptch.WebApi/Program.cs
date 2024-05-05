using Dsptch.DependencyInjection;
using Dsptch.Decorators;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Dsptch.WebApi.Decorators;
using Dsptch.WebApi.Queries;
using Dsptch.WebApi.Commands;
using Dsptch.WebApi.Models;
using Dsptch.WebApi.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/products/{id}", async (IDispatcher dispatcher, Guid id) =>
{
    Product? product = await dispatcher.Dispatch<GetProductByIdQuery, Product?>(new GetProductByIdQuery(id));

    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.MapGet("products", async (IDispatcher dispatcher, string name) =>
{
    return Results.Ok(await dispatcher.Dispatch<GetProductByNameQuery, List<Product>>(new GetProductByNameQuery(name)));
});

app.MapPost("/products", async (IDispatcher dispatcher, CreateProductCommand command) =>
{
    return Results.Ok(await dispatcher.Dispatch<CreateProductCommand, Guid>(command));
});

app.MapPost("/cart", async (IDispatcher dispatcher, AddProductToCartRequest request) =>
{
    return Results.Ok(await dispatcher.Dispatch<AddProductToCartRequest, bool>(request));
});

app.Run();
