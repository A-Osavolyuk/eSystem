using Scalar.AspNetCore;

namespace eShop.SmsSender.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void MapApiServices(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        app.MapDefaultEndpoints();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
    }
}