namespace eSystem.EmailSender.Api.Extensions;

public static class WebApplicationBuilder
{
    public static void MapApiServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.UseExceptionHandler();
        app.MapOpenApi();
        app.MapScalarApiReference();
    }
}