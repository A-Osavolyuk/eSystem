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
                RouteId = "profile-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Profile/{**catch-all}" }
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
                RouteId = "cart-route", ClusterId = "cart-cluster",
                Match = new RouteMatch { Path = "/api/v1/Carts/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "favorites-route", ClusterId = "cart-cluster",
                Match = new RouteMatch { Path = "/api/v1/Favorites/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "brand-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Brands/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "product-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Products/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "seller-route", ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/v1/Seller/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "comment-route", ClusterId = "review-cluster",
                Match = new RouteMatch { Path = "/api/v1/Comments/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "files-route", ClusterId = "files-cluster",
                Match = new RouteMatch { Path = "/api/v1/Files/{**catch-all}" }
            },
        };

        var clusterConfigs = new[]
        {
            new ClusterConfig
            {
                ClusterId = "security-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["security-destination"] = new DestinationConfig { Address = "https://localhost:40201/" }
                }
            },
            new ClusterConfig
            {
                ClusterId = "email-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["review-destination"] = new DestinationConfig { Address = "https://localhost:40202/" }
                }
            },
            new ClusterConfig
            {
                ClusterId = "files-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["files-destination"] = new DestinationConfig { Address = "https://localhost:40203/" }
                }
            },
            new ClusterConfig
            {
                ClusterId = "product-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["product-destination"] = new DestinationConfig { Address = "https://localhost:40204/" }
                }
            },
            new ClusterConfig
            {
                ClusterId = "cart-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["cart-destination"] = new DestinationConfig { Address = "https://localhost:40205/" }
                }
            },
            new ClusterConfig
            {
                ClusterId = "review-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["review-destination"] = new DestinationConfig { Address = "https://localhost:40206/" }
                }
            },
            new ClusterConfig
            {
                ClusterId = "sms-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["review-destination"] = new DestinationConfig { Address = "https://localhost:40207/" }
                }
            },
            new ClusterConfig
            {
                ClusterId = "telegram-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["review-destination"] = new DestinationConfig { Address = "https://localhost:40208/" }
                }
            },
        };
        
        builder.Services.AddReverseProxy().LoadFromMemory(routeConfigs, clusterConfigs);
    }
}