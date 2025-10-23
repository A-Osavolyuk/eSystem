using eSystem.ServiceDefaults;
using Scalar.AspNetCore;

namespace eSystem.Storage.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void MapAppServices(this WebApplication app)
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