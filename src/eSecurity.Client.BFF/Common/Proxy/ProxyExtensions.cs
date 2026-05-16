using System.Net.Http.Headers;
using eSecurity.Client.BFF.Security.Authentication.Token;
using eSystem.Core.Http.Constants;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

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
                ]
            ).AddTransforms(context =>
            {
                context.AddRequestTransform(async request =>
                {
                    var httpContent = request.HttpContext;
                    if (httpContent.User.Identity?.IsAuthenticated == true)
                    {
                        var tokenHandler = httpContent.RequestServices.GetRequiredService<ITokenHandler>();
                        var accessToken = await tokenHandler.GetTokenAsync();
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            var header = new AuthenticationHeaderValue(AuthenticationTypes.Bearer, accessToken);
                            request.ProxyRequest.Headers.Authorization = header;
                        }
                    }
                });
            });
    }
}