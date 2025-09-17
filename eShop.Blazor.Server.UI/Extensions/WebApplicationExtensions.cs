using eShop.Blazor.Server.UI.Components;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Localization;

namespace eShop.Blazor.Server.UI.Extensions;

public static class WebApplicationExtensions
{
    public static void MapAppServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();

        app.UseStaticWebAssets();
        app.UseRouting();
        app.MapControllers();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        app.UseStatusCodePagesWithRedirects("/Error?code={0}");
        app.UseLocalization();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
    }

    private static void UseLocalization(this WebApplication app)
    {
        var supportedCultures = new[] { "en-US", "ru-RU", "uk-UA" };

        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en-US"),
            ApplyCurrentCultureToResponseHeaders = true
        };

        localizationOptions.AddSupportedCultures(supportedCultures);
        localizationOptions.AddSupportedUICultures(supportedCultures);
        localizationOptions.RequestCultureProviders = [new CookieRequestCultureProvider()];

        app.UseRequestLocalization(localizationOptions);
    }

    private static void UseStaticWebAssets(this WebApplication app)
    {
        app.UseStaticFiles();
        app.MapStaticAssets();
        StaticWebAssetsLoader.UseStaticWebAssets(app.Environment, app.Configuration);
    }
}