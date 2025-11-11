using eSecurity.Client.Components;
using eSystem.ServiceDefaults;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

namespace eSecurity.Client.Extensions;

public static class WebApplicationExtensions
{
    public static void MapServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.UseStaticWebAssets();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseAntiforgery();
        app.UseStatusCodePagesWithRedirects("/not-found");
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
    }
    
    private static void UseStaticWebAssets(this WebApplication app)
    {
        app.UseStaticFiles();
        app.MapStaticAssets();
        StaticWebAssetsLoader.UseStaticWebAssets(app.Environment, app.Configuration);
    }
}