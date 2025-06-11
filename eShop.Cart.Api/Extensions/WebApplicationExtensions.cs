using Scalar.AspNetCore;

namespace eShop.Cart.Api.Extensions;

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
        app.MapGrpcService<CartServer>();
    }
}