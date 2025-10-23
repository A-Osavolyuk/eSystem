using eSystem.Application.Data;
using eSystem.Auth.Api.Data;

namespace eSystem.Auth.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static async Task MapApiServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        app.MapOpenApi();
        app.MapScalarApiReference();
        await app.ConfigureDatabaseAsync<AuthDbContext>();

        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
    }
}