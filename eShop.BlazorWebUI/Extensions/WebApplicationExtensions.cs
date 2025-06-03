using Microsoft.AspNetCore.Localization;

namespace eShop.BlazorWebUI.Extensions;

public static class WebApplicationExtensions
{
    public static void MapAppServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.MapStaticAssets();
        app.UseHttpsRedirection();
        app.UseRouting();
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
}