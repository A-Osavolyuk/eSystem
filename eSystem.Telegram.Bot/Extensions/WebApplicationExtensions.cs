using Scalar.AspNetCore;

namespace eSystem.Telegram.Bot.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void MapApiServices()
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.MapDefaultEndpoints();
            app.UseAuthorization();
            app.MapControllers();
            app.UseExceptionHandler();
        }
    }
}