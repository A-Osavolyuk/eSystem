namespace eSystem.EmailSender.Api.Extensions;

public static class WebApplicationBuilder
{
    extension(WebApplication app)
    {
        public void MapApiServices()
        {
            app.MapDefaultEndpoints();
            app.UseExceptionHandler();
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    }
}