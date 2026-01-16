using eSecurity.Client.Common.Cache;
using eSecurity.Client.Common.Confirmation;
using eSecurity.Client.Common.Http;
using eSecurity.Client.Common.JS;
using eSecurity.Client.Common.State;
using eSecurity.Client.Security;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Gateway;
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
            builder.AddRedisCache();
            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient("Raw");
            builder.Services.AddHttpClient<IApiClient, ApiClient>();
            builder.Services.AddScoped<NavigationContext>();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddSecurity();
            builder.Services.AddConfirmation();
            builder.Services.AddJs();
            builder.Services.AddState();
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