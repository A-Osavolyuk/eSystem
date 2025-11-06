using eSecurity.Components;
using eSystem.Core.Data;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

namespace eSecurity.Extensions;

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
        app.UseAntiforgery();
        app.UseStaticWebAssets();
        app.UseStatusCodePagesWithRedirects("/not-found");
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        await app.ConfigureDatabaseAsync<AuthDbContext>();
    }
    
    private static void UseStaticWebAssets(this WebApplication app)
    {
        app.UseStaticFiles();
        app.MapStaticAssets();
        StaticWebAssetsLoader.UseStaticWebAssets(app.Environment, app.Configuration);
    }
}