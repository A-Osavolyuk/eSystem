using eSystem.ServiceDefaults;
using Scalar.AspNetCore;

namespace eSystem.Storage.Api.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void MapAppServices()
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.UseAuthorization();
            app.MapControllers();
            app.MapDefaultEndpoints();
            app.UseExceptionHandler();

            app.Run();
        }
    }
}