using eShop.ServiceDefaults;

namespace eShop.Proxy.Extensions;

public static class WebApplicationExtensions
{
    public static void MapApiServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.UseCors();
        app.MapReverseProxy(proxyPipeline =>
        {
            proxyPipeline.UseAuthorization();
        });
    }
}