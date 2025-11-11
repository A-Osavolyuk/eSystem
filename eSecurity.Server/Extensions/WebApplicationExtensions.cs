using eSecurity.Server.Data;
using eSystem.Core.Data;

namespace eSecurity.Server.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MapServicesAsync(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();

        await app.ConfigureDatabaseAsync<AuthDbContext>();
    }
}