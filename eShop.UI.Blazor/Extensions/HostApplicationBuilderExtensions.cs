using MudBlazor.Services;
using MudExtensions.Services;

namespace eShop.BlazorWebUI.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddAppServices(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.AddInfrastructureLayer();
        builder.AddValidation();
        builder.Services.AddLocalization(cfg => cfg.ResourcesPath = "Resources");
        
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Configuration:Logging"));
        builder.Services.AddMudExtensions();
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        builder.AddRouting(cfg =>
        {
            cfg.OnNotFound = "/error?code=404";
            cfg.OnForbidden = "/error?code=403";
            cfg.OnUnauthorized = "/error?code=401";

            cfg.Pages =
            [
                new() { Routes = ["/products"] },
                new() { Routes = ["/products/create"], RequiredRoles = ["Seller"]},
                new() { Routes = ["/account/login"] },
                new() { Routes = ["/account/oauth/login"] },
                new() { Routes = ["/account/2fa/login"] },
                new() { Routes = ["/account/locked-out"], RequireAuthorization = true },
                new() { Routes = ["/account/unlock"], RequireAuthorization = true },
                new() { Routes = ["/account/register"] },
                new() { Routes = ["/account/profile"], RequireAuthorization = true },
                new() { Routes = ["/account/security"], RequireAuthorization = true },
                new() { Routes = ["/account/settings"], RequireAuthorization = true },
                new() { Routes = ["/account/orders"], RequireAuthorization = true },
                new() { Routes = ["/account/orders-history"], RequireAuthorization = true },
                new() { Routes = ["/account/phone-number/add"], RequireAuthorization = true },
                new() { Routes = ["/account/phone-number/change"], RequireAuthorization = true },
                new() { Routes = ["/account/phone-number/reset"], RequireAuthorization = true },
                new() { Routes = ["/account/email/change"], RequireAuthorization = true },
                new() { Routes = ["/account/email/reset"], RequireAuthorization = true },
                new() { Routes = ["/account/email/verify"], RequireAuthorization = true },
                new() { Routes = ["/account/password/reset"], RequireAuthorization = true },
            ];
        });
    }

    private static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
    }
}