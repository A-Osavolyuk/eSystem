using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using eShop.Application.Attributes;
using eShop.Application.Documentation.Transformers;
using eShop.Application.Middlewares;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace eShop.Application.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddDocumentation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerTokenTransformer>();
        });
    }

    public static void AddVersioning(this IHostApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
        {
            const string key = "api-version";
            
            options.ReportApiVersions = true;
            options.DefaultApiVersion = ApiVersion.Default;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader(key),
                new HeaderApiVersionReader(key));
        });

        builder.Services.AddVersionedApiExplorer(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = ApiVersion.Default;
            options.SubstituteApiVersionInUrl = true;
            options.GroupNameFormat = "'v'V";
        });
    }

    public static void AddJwtAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            const string audiencePath = "JWT:Audience";
            const string issuerPath = "JWT:Issuer";
            const string keyPath = "JWT:Secret";
            
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = builder.Configuration[audiencePath],
                ValidIssuer = builder.Configuration[issuerPath],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration[keyPath]!))
            };
        });
    }
    
    public static void AddLogging(this IHostApplicationBuilder builder)
    {
        const string key = "Configuration:Logging";
        builder.Logging.AddConfiguration(builder.Configuration.GetSection(key));
    }

    public static void AddExceptionHandler(this IHostApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
    }
    
    public static void AddHttpClient<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>
        (this IServiceCollection services, ServiceLifetime lifetime)
        where TImplementation : class, TService
        where TService : class
    {
        services.AddHttpClient<TService, TImplementation>();
        var serviceDescriptor = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);
        services.Add(serviceDescriptor);
    }

    private static void AddHttpClient(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        var addHttpClientMethod = typeof(HttpClientFactoryServiceCollectionExtensions)
            .GetMethods()
            .First(m => m is { Name: "AddHttpClient", IsGenericMethodDefinition: true } 
                        && m.GetGenericArguments().Length == 2 
                        && m.GetParameters().Length == 1);

        var genericMethod = addHttpClientMethod.MakeGenericMethod(serviceType, implementationType);

        genericMethod.Invoke(null, [services]);

    }

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