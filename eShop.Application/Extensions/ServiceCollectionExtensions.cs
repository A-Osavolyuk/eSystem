using System.Diagnostics.CodeAnalysis;
using eShop.Domain.Common.Messaging;

namespace eShop.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddHttpClient<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>
        (this IServiceCollection services, ServiceLifetime lifetime)
        where TImplementation : class, TService
        where TService : class
    {
        services.AddHttpClient<TService, TImplementation>();
        var serviceDescriptor = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);
        services.Add(serviceDescriptor);
    }

    public static void AddHttpClient(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        var addHttpClientMethod = typeof(HttpClientFactoryServiceCollectionExtensions)
            .GetMethods()
            .First(m => m is { Name: "AddHttpClient", IsGenericMethodDefinition: true } 
                        && m.GetGenericArguments().Length == 2 
                        && m.GetParameters().Length == 1);

        var genericMethod = addHttpClientMethod.MakeGenericMethod(serviceType, implementationType);

        genericMethod.Invoke(null, [services]);

    }
}