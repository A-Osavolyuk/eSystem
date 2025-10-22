using eShop.Application.Data;
using Scalar.AspNetCore;

namespace eShop.Comments.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MapApiServices(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        await app.ConfigureDatabaseAsync<AppDbContext>();
        app.MapDefaultEndpoints();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
    }
}