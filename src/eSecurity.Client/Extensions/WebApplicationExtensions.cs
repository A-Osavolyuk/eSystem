using eSecurity.Client.Components;
using eSystem.ServiceDefaults;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

namespace eSecurity.Client.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void MapServices()
        {
            app.MapDefaultEndpoints();
            app.UseStaticWebAssets();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseStatusCodePagesWithRedirects("/not-found");
            app.UseAntiforgery();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
        }

        private void UseStaticWebAssets()
        {
            app.UseStaticFiles();
            app.MapStaticAssets();
            StaticWebAssetsLoader.UseStaticWebAssets(app.Environment, app.Configuration);
        }
    }
}