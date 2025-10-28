using System.Reflection;
using eSystem.Core.Common.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Attributes;

public static class AttributesExtensions
{
    public static void AddServices<TMarker>(this IHostApplicationBuilder builder)
    {
        var implementations = typeof(TMarker).Assembly.GetTypes()
            .Where(x => x is { IsClass: true, IsAbstract: false } 
                        && x.GetCustomAttribute<InjectableAttribute>() is not null);

        foreach (var implementation in implementations)
        {
            var attribute = implementation.GetCustomAttribute<InjectableAttribute>()!;

            var serviceType = attribute.Type;
            var lifetime = attribute.Lifetime;
            var key = attribute.Key;
            
            if (string.IsNullOrEmpty(key))
            {
                var service = new ServiceDescriptor(serviceType, implementation, lifetime);
                builder.Services.Add(service);
            }
            else
            {
                var service = new ServiceDescriptor(serviceType, key, implementation, lifetime);
                builder.Services.Add(service);
            }
        }
    }
}