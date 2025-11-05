using Microsoft.AspNetCore.Hosting.StaticWebAssets;

namespace eSecurity.Extensions;

public static class WebApplicationExtensions
{
    public static void UseStaticWebAssets(this WebApplication app)
    {
        app.UseStaticFiles();
        app.MapStaticAssets();
        StaticWebAssetsLoader.UseStaticWebAssets(app.Environment, app.Configuration);
    }
}