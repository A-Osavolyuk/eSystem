using eSystem.ServiceDefaults;

namespace eSystem.MessageBus.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void MapServices()
        {
            app.MapDefaultEndpoints();
        }
    }

}