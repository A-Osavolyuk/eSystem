using eShop.Application.Data;
using Scalar.AspNetCore;

namespace eShop.Product.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MapApiServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        app.MapOpenApi();
        app.MapScalarApiReference();
        await app.ConfigureDatabaseAsync<AppDbContext>();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
    }
}