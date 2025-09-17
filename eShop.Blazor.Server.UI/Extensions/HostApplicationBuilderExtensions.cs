using eShop.Blazor.Server.Application;
using eShop.Blazor.Server.Infrastructure;
using MudBlazor.Services;
using MudExtensions.Services;

namespace eShop.Blazor.Server.UI.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddAppServices(this WebApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.AddInfrastructureLayer();
        builder.AddValidation();
        
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Configuration:Logging"));
        
        builder.Services.AddControllers();
        builder.Services.AddLocalization(cfg => cfg.ResourcesPath = "Resources");
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
                new() { Routes = ["/error"] },
                new() { Routes = ["/products"] },
                new() { Routes = ["/products/create"], RequiredRoles = ["Seller"] },
                new() { Routes = ["/account/login"] },
                new() { Routes = ["/account/recover"] },
                new() { Routes = ["/account/register"] },
                new() { Routes = ["/account/passkey/sign-in"] },
                new() { Routes = ["/account/passkey/remove"], RequireAuthorization = true },
                new() { Routes = ["/account/oauth/sign-in"] },
                new() { Routes = ["/account/oauth/signed-up"] },
                new() { Routes = ["/account/oauth/fallback"] },
                new() { Routes = ["/account/linked-account/disconnect"], RequireAuthorization = true },
                new() { Routes = ["/account/linked-account/allow"], RequireAuthorization = true },
                new() { Routes = ["/account/linked-account/disallow"], RequireAuthorization = true },
                new() { Routes = ["/account/2fa/login"] },
                new() { Routes = ["/account/2fa/authenticator/enable"], RequireAuthorization = true },
                new() { Routes = ["/account/2fa/provider/enable"], RequireAuthorization = true },
                new() { Routes = ["/account/2fa/provider/disable"], RequireAuthorization = true },
                new() { Routes = ["/account/locked-out"] },
                new() { Routes = ["/account/unlock"] },
                new() { Routes = ["/account/profile"], RequireAuthorization = true },
                new() { Routes = ["/account/recovery"], RequireAuthorization = true },
                new() { Routes = ["/account/settings"], RequireAuthorization = true },
                new() { Routes = ["/account/orders"], RequireAuthorization = true },
                new() { Routes = ["/account/orders-history"], RequireAuthorization = true },
                new() { Routes = ["/account/phone-numbers"], RequireAuthorization = true },
                new() { Routes = ["/account/phone-number/add"], RequireAuthorization = true },
                new() { Routes = ["/account/phone-number/change"], RequireAuthorization = true },
                new() { Routes = ["/account/phone-number/reset"], RequireAuthorization = true },
                new() { Routes = ["/account/phone-number/verify"], RequireAuthorization = true },
                new() { Routes = ["/account/phone-number/remove"], RequireAuthorization = true },
                new() { Routes = ["/account/emails"], RequireAuthorization = true },
                new() { Routes = ["/account/email/add"], RequireAuthorization = true },
                new() { Routes = ["/account/email/change"], RequireAuthorization = true },
                new() { Routes = ["/account/email/reset"], RequireAuthorization = true },
                new() { Routes = ["/account/email/verify"], RequireAuthorization = true },
                new() { Routes = ["/account/email/remove"], RequireAuthorization = true },
                new() { Routes = ["/account/secondary-email/add"], RequireAuthorization = true },
                new() { Routes = ["/account/secondary-email/remove"], RequireAuthorization = true },
                new() { Routes = ["/account/secondary-email/verify"], RequireAuthorization = true },
                new() { Routes = ["/account/recovery-email/add"], RequireAuthorization = true },
                new() { Routes = ["/account/recovery-email/change"], RequireAuthorization = true },
                new() { Routes = ["/account/recovery-email/verify"], RequireAuthorization = true },
                new() { Routes = ["/account/recovery-email/remove"], RequireAuthorization = true },
                new() { Routes = ["/account/security"], RequireAuthorization = true },
                new() { Routes = ["/account/password/forgot"] },
                new() { Routes = ["/account/password/reset"], RequireAuthorization = true },
                new() { Routes = ["/account/devices"], RequireAuthorization = true },
                new() { Routes = ["/account/device/trust"] },
                new() { Routes = ["/account/device/verify"], RequireAuthorization = true },
                new() { Routes = ["/account/device/block"], RequireAuthorization = true },
                new() { Routes = ["/account/device/unblock"], RequireAuthorization = true },
            ];
        });
    }
}