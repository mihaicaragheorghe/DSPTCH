using Dsptch.DependencyInjection;
using Dsptch.Decorators;
using Dsptch.WebApi.Commands;
using Dsptch.WebApi.Queries;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Dsptch.WebApi.Decorators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddDsptch(opts =>
{
    opts.RegisterServicesFromAssemblyContaining<Program>();

    // Registering dispatchers using the RegisterQueryDispatcher and RegisterCommandDispatcher methods
    opts.RegisterDispatcherDecorator(typeof(LoggingDecorator<,>));
    opts.RegisterDispatcherDecorator(typeof(CachingDecorator<,>));
    opts.RegisterDispatcherDecorator(typeof(CustomQueryDecorator<,>));
});

// Registering dispatchers manually
builder.Services.TryAddTransient(typeof(IDispatcherDecorator<,>), typeof(LoggingDecorator<,>));
builder.Services.TryAddTransient(typeof(IDispatcherDecorator<,>), typeof(CachingDecorator<,>));
builder.Services.TryAddTransient(typeof(IDispatcherDecorator<,>), typeof(CustomQueryDecorator<,>));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/hello", async (IDispatcher dispatcher) =>
{
    var command = new SampleCommand(42, "John Doe");

    return await dispatcher.Dispatch<SampleCommand, string>(command);
});

app.MapGet("/hello/{name}", async (IDispatcher dispatcher, string name) =>
{
    var query = new SampleQuery(name);

    return await dispatcher.Dispatch<SampleQuery, string>(query);
});

app.MapGet("/hello/custom/{name}", async (IDispatcher dispatcher, string name) =>
{
    var query = new QueryWithCustomDecorator(name);

    return await dispatcher.Dispatch<QueryWithCustomDecorator, string>(query);
});

app.MapGet("/hello/{id:int}/{name}", async (IDispatcher dispatcher, int id, string name) =>
{
    var request = new SampleRequest(id, name);

    return await dispatcher.Dispatch<SampleRequest, string>(request);
});

app.Run();
