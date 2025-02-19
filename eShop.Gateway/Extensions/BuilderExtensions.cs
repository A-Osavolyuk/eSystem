using eShop.Application.Extensions;
using eShop.ServiceDefaults;

namespace eShop.Gateway.Extensions;

public static class BuilderExtensions
{
    public static void AppApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.AddJwtAuthentication();
        builder.AddLogging();
        builder.AddReverseProxy();
        builder.Services.AddOpenApi();
    }

    private static void AddReverseProxy(this IHostApplicationBuilder builder)
    {
        builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    }
}