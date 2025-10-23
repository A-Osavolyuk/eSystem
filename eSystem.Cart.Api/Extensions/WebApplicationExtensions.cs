using Scalar.AspNetCore;

namespace eSystem.Cart.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void MapApiServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
    }
}