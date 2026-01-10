using Scalar.AspNetCore;

namespace eSystem.SmsSender.Api.Extensions;

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