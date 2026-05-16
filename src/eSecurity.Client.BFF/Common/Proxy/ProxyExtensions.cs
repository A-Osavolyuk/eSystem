using Yarp.ReverseProxy.Configuration;

namespace eSecurity.Client.BFF.Common.Proxy;

public static class ProxyExtensions
{
    public static void AddProxy(this IHostApplicationBuilder builder)
    {
        builder.Services.AddReverseProxy()
            .LoadFromMemory(
                [
                    new RouteConfig
                    {
                        RouteId = "proxy-route", ClusterId = "proxy-cluster",
                        Match = new RouteMatch { Path = "/api/v1/{**catch-all}" }
                    }
                ],
                [
                    new ClusterConfig
                    {
                        ClusterId = "proxy-cluster",
                        Destinations = new Dictionary<string, DestinationConfig>
                        {
                            ["proxy-destination"] = new()
                            {
                                Address = builder.Configuration["PROXY_HTTPS"]!
                            }
                        }
                    }
                ]);
    }
}