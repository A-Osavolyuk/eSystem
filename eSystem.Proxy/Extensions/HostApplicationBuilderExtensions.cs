using eSystem.Core.Common.Logging;
using eSystem.Core.Security.Authentication;
using eSystem.ServiceDefaults;
using Yarp.ReverseProxy.Configuration;

namespace eSystem.Proxy.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AppApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
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
                p.WithOrigins("http://localhost:5501");
                p.AllowCredentials();
            });
        });
    }

    private static void AddReverseProxy(this IHostApplicationBuilder builder)
    {
        var routeConfigs = new[]
        {
            new RouteConfig
            {
                RouteId = "account-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Account/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "connect-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Connect/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "device-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Device/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "email-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Email/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "linked-account-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/LinkedAccount/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "oauth-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/OAuth/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "passkey-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Passkey/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "password-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Password/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "phone-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Phone/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "2fa-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/TwoFactor/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "user-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/User/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "verification-route", ClusterId = "security-cluster",
                Match = new RouteMatch { Path = "/api/v1/Verification/{**catch-all}" }
            },
            new RouteConfig
            {
                RouteId = "files-route", ClusterId = "files-cluster",
                Match = new RouteMatch { Path = "/api/v1/Files/{**catch-all}" }
            }
        };

        var configuration = builder.Configuration;

        var clusterConfigs = new[]
        {
            new ClusterConfig
            {
                ClusterId = "security-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["security-destination"] = new() { Address = configuration["services:e-security-server:http:0"]! }
                }
            },
            new ClusterConfig
            {
                ClusterId = "files-cluster",
                Destinations = new Dictionary<string, DestinationConfig>()
                {
                    ["files-destination"] = new() { Address = configuration["services:storage-api:http:0"]! }
                }
            }
        };
        
        builder.Services.AddReverseProxy().LoadFromMemory(routeConfigs, clusterConfigs);
    }
}