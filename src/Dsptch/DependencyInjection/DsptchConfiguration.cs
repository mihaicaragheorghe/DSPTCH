using System.Reflection;

using Dsptch.Decorators;

using Microsoft.Extensions.DependencyInjection;

namespace Dsptch.DependencyInjection;

public class DsptchConfiguration
{
    internal List<Assembly> Assemblies { get; } = [];
    public List<ServiceDescriptor> Decorators { get; } = [];
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

    public DsptchConfiguration RegisterServicesFromAssembly(Assembly assembly)
    {
        Assemblies.Add(assembly);

        return this;
    }

    public DsptchConfiguration RegisterServicesFromAssemblies(Assembly[] assemblies)
    {
        Assemblies.AddRange(assemblies);

        return this;
    }

    public DsptchConfiguration RegisterServicesFromAssemblyContaining(Type type)
        => RegisterServicesFromAssembly(type.Assembly);

    public DsptchConfiguration RegisterServicesFromAssemblyContaining<T>()
        => RegisterServicesFromAssembly(typeof(T).Assembly);

    public DsptchConfiguration RegisterDispatcherDecorator<TDispatcher>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        => RegisterDispatcherDecorator(typeof(TDispatcher), lifetime);

    public DsptchConfiguration RegisterDispatcherDecorator(Type dispatcherType, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        if (!dispatcherType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDispatcherDecorator<,>)))
        {
            throw new ArgumentException("The dispatcher type must implement IQueryDispatcher<TQuery, TResult>", nameof(dispatcherType));
        }

        Decorators.Add(new ServiceDescriptor(typeof(IDispatcherDecorator<,>), dispatcherType, lifetime));

        return this;
    }
}
