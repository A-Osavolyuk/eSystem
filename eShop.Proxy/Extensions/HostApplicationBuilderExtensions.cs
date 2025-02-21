using eShop.Application.Extensions;
using eShop.ServiceDefaults;

namespace eShop.Proxy.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AppApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.AddJwtAuthentication();
        builder.AddLogging();
        builder.AddReverseProxy();
        builder.Services.AddOpenApi();
        builder.Configuration.AddJsonFile("proxy.json", optional: false, reloadOnChange: true);
    }

    private static void AddReverseProxy(this IHostApplicationBuilder builder)
    {
        builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    }
}