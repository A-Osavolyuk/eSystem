using eShop.ServiceDefaults;

namespace eShop.MessageBus.Extensions;

public static class WebApplicationExtensions
{
    public static void MapServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();
    }

}