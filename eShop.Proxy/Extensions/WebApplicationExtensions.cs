using eShop.ServiceDefaults;
using Scalar.AspNetCore;

namespace eShop.Proxy.Extensions;

public static class WebApplicationExtensions
{
    public static void MapApiServices(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.MapDefaultEndpoints();
        app.MapReverseProxy(proxyPipeline =>
        {
            proxyPipeline.UseAuthorization();
        });
    }
}