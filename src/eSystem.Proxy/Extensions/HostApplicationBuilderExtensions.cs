using eSystem.ServiceDefaults;
using Yarp.ReverseProxy.Configuration;

namespace eSystem.Proxy.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AppApiServices()
        {
            builder.AddServiceDefaults();
            builder.AddReverseProxy();
            builder.AddCors();
        }

        private void AddCors()
        {
            builder.Services.AddCors(o =>
            {
                o.AddDefaultPolicy(p =>
                {
                    p.AllowAnyHeader();
                    p.AllowAnyMethod();
                    p.WithOrigins("https://localhost:6501");
                    p.AllowCredentials();
                });
            });
        }

        private void AddReverseProxy()
        {
            var routeConfigs = new[]
            {
                new RouteConfig
                {
                    RouteId = "consent-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/consent/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "account-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/account/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "connect-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/connect/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "device-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/device/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "email-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/email/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "linked-account-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/linked-account/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "oauth-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/oauth/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "passkey-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/software-key/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "password-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/password/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "phone-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/phone/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "2fa-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/two-factor/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "user-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/user/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "verification-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/verification/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "files-route", ClusterId = "files-cluster",
                    Match = new RouteMatch { Path = "/api/v1/files/{**catch-all}" }
                }
            };

            var configuration = builder.Configuration;

            var clusterConfigs = new[]
            {
                new ClusterConfig
                {
                    ClusterId = "security-cluster",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        ["security-destination"] = new() { Address = configuration["E_SECURITY_IDP_HTTPS"]! }
                    }
                },
                new ClusterConfig
                {
                    ClusterId = "files-cluster",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        ["files-destination"] = new() { Address = configuration["STORAGE_API_HTTPS"]! }
                    }
                }
            };
        
            builder.Services.AddReverseProxy().LoadFromMemory(routeConfigs, clusterConfigs);
        }
    }
}