using System.Reflection;

using Dsptch.Decorators;
using Dsptch.Handlers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dsptch.DependencyInjection;

public static class DependencyInjectionRegistry
{
    /// <summary>
    /// Registers the Dsptch services with the provided configuration.<br/>
    /// - Will scan the provided assemblies for IRequestHandler implementations and register them with the service collection with the provided lifetime.<br/>
    /// - Will also register the IDispatcher service with the service collection as a transient service.<br/>
    /// - Will not register any decorators.<br/>
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration action</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDsptch(this IServiceCollection services,
        Action<DsptchConfiguration> configuration)
    {
        var config = new DsptchConfiguration();

        configuration.Invoke(config);

        return services.AddDsptch(config);
    }


    /// <summary>
    /// Registers the Dsptch services with the provided configuration.<br/>
    /// - Will scan the provided assemblies for IRequestHandler implementations and register them with the service collection with the provided lifetime.<br/>
    /// - Will also register the IDispatcher service with the service collection as a transient service.<br/>
    /// - Will not register any decorators.<br/>
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
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

        services.TryAdd(new ServiceDescriptor(typeof(IDispatcher), typeof(Dispatcher), configuration.Lifetime));

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