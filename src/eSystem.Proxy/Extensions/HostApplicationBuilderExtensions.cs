using eSystem.Core.Common.Logging;
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
            builder.AddLogging();
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
                    Match = new RouteMatch { Path = "/api/v1/Consent/{**catch-all}" }
                },
                new RouteConfig
                {
                    RouteId = "username-route", ClusterId = "security-cluster",
                    Match = new RouteMatch { Path = "/api/v1/Username/{**catch-all}" }
                },
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
                        ["security-destination"] = new() { Address = configuration["E_SECURITY_SERVER_HTTPS"]! }
                    }
                },
                new ClusterConfig
                {
                    ClusterId = "files-cluster",
                    Destinations = new Dictionary<string, DestinationConfig>()
                    {
                        ["files-destination"] = new() { Address = configuration["STORAGE_API_HTTPS"]! }
                    }
                }
            };
        
            builder.Services.AddReverseProxy().LoadFromMemory(routeConfigs, clusterConfigs);
        }
    }
}