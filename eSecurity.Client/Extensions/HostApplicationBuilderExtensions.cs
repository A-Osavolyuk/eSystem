using eSecurity.Client.Common.Confirmation;
using eSecurity.Client.Common.Http;
using eSecurity.Client.Common.JS;
using eSecurity.Client.Common.State;
using eSecurity.Client.Common.Storage;
using eSecurity.Client.Security;
using eSecurity.Client.Security.Authentication.Oidc;
using eSecurity.Client.Security.Authentication.TwoFactor;
using eSecurity.Client.Security.Authorization.Access.Verification;
using eSecurity.Client.Security.Authorization.Devices;
using eSecurity.Client.Security.Authorization.LinkedAccounts;
using eSecurity.Client.Security.Authorization.OAuth;
using eSecurity.Client.Security.Credentials.PublicKey;
using eSecurity.Client.Security.Identity;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Validation;
using eSystem.ServiceDefaults;
using MudBlazor;
using MudBlazor.Services;
using MudExtensions.Services;
using NavigationContext = eSecurity.Core.Common.Routing.NavigationContext;

namespace eSecurity.Client.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.AddServiceDefaults();
            builder.AddValidation<IAssemblyMarker>();
        
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IApiClient, ApiClient>();
            builder.Services.AddScoped<NavigationContext>();
        
            builder.Services.AddSecurity();
            builder.Services.AddConfirmation();
            builder.Services.AddJs();
            builder.Services.AddState();
            builder.Services.AddStorage();
            builder.Services.AddGateway();
            builder.Services.AddLocalization(cfg => cfg.ResourcesPath = "Resources");
            builder.Services.AddMudExtensions();
            builder.Services.AddControllers();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

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
        }
    }
}