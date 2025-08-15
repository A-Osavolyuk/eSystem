using eShop.Application.Extensions;
using eShop.ServiceDefaults;
using Yarp.ReverseProxy.Configuration;

namespace eShop.Proxy.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AppApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.AddJwtAuthentication();
        builder.AddLogging();
        builder.AddReverseProxy();
        builder.AddCors();
    }

    private static void AddCors(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(o =>
        {
            o.AddDefaultPolicy(p =>
            {
                p.AllowAnyHeader();
                p.AllowAnyMethod();
                p.AllowAnyOrigin();
            });
        });
    }

    private static void AddReverseProxy(this IHostApplicationBuilder builder)
    {
        var routeConfigs = new[]
        {
            new RouteConfig
            {
                RouteId = "security-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Security/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "permission-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Permissions/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "roles-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Roles/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "two-factor-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/TwoFactor/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "lockout-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Lockout/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "oauth-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/OAuth/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "users-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Users/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "providers-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Providers/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "device-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Device/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "files-route", ClusterId = "files-cluster",
                Match = new RouteMatch { Path = "/api/v1/Files/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "product-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Products/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "types-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Types/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "units-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Units/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "price-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Price/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "categories-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Category/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "currency-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Currency/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "brand-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Brand/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "supplier-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Supplier/{**catch-all}" }
            },
        };

        var configuration = builder.Configuration;

        var clusterConfigs = new[]
        {
            new ClusterConfig
            {
                ClusterId = "security-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["security-destination"] = new() { Address = configuration["services:auth-api:http:0"]! }
                }
            },
            new ClusterConfig
            {
                ClusterId = "files-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["files-destination"] = new() { Address = configuration["services:storage-api:http:0"]! }
                }
            },
            new ClusterConfig
            {
                ClusterId = "product-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["product-destination"] = new() { Address = configuration["services:product-api:http:0"]! }
                }
            },
            new ClusterConfig
            {
                ClusterId = "cart-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["cart-destination"] = new() { Address = configuration["services:cart-api:http:0"]! }
                }
            },
            new ClusterConfig
            {
                ClusterId = "comment-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["comment-destination"] = new() { Address = configuration["services:comment-api:http:0"]! }
                }
            },
        };
        
        builder.Services.AddReverseProxy().LoadFromMemory(routeConfigs, clusterConfigs);
    }
}