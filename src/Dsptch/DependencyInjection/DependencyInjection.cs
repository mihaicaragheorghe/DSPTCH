using System.Reflection;

using Dsptch.Decorators;
using Dsptch.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Dsptch.DependencyInjection;

public static class DependencyInjectionRegistry
{
    public static IServiceCollection AddDsptch(this IServiceCollection services,
        Action<DsptchConfiguration> configuration)
    {
        var config = new DsptchConfiguration();

        configuration.Invoke(config);

        return services.AddDsptch(config);
    }

    public static IServiceCollection AddDsptch(this IServiceCollection services,
        DsptchConfiguration configuration)
    {
        if (configuration.Assemblies.Count == 0)
        {
            throw new InvalidOperationException("No assemblies were registered.");
        }

        foreach (var assembly in configuration.Assemblies)
        {
            RegisterRequestHandlers(services, assembly, configuration.Lifetime);
        }

        services.AddTransient<IDispatcher, Dispatcher>();

        foreach (var decorator in configuration.Decorators)
        {
            services.Add(decorator);
        }

        return services;
    }

    private static void RegisterRequestHandlers(IServiceCollection services, Assembly assembly, ServiceLifetime lifetime)
    {
        var handlerType = typeof(IRequestHandler<,>);
        var handlerImplementations = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType));

        foreach (var handlerImplType in handlerImplementations)
        {
            var handlerInterface = handlerImplType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType);

            services.Add(new ServiceDescriptor(handlerInterface, handlerImplType, lifetime));
        }
    }
}