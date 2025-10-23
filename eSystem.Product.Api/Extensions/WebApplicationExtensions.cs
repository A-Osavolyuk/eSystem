using eSystem.Application.Data;
using eSystem.Product.Api.Data;
using Scalar.AspNetCore;

namespace eSystem.Product.Api.Extensions;

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