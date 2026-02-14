using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Mediator;

public static class MediatorExtensions
{
    public static void AddMediator<TAssemblyMarker>(this IServiceCollection services)
        where TAssemblyMarker : notnull
    {
        var types = typeof(TAssemblyMarker).Assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false });
        
        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                             i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

            foreach (var @interface in interfaces)
            {
                services.AddTransient(@interface, type);
            }
        }

        services.AddScoped<ISender, Sender>();
    }
}