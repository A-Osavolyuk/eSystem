using System.Reflection;
using eShop.Application.Common.Http;
using Microsoft.Extensions.Hosting;

namespace eShop.Application.Attributes;

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
            var withHttpClient = attribute.WithHttpClient;
            var key = attribute.Key;

            if (withHttpClient)
            {
                builder.Services.AddHttpClient(serviceType, implementation);
            }
            
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