using eSecurity.Client.Common.Configurations;
using eSecurity.Client.Common.Confirmation;
using eSecurity.Client.Common.Http;
using eSecurity.Client.Common.JS;
using eSecurity.Client.Common.State;
using eSecurity.Client.Security;
using eSystem.Core.Gateway;
using eSystem.Core.Server.Validation;
using eSystem.Core.Server.Versioning;
using eSystem.Core.Validation;
using eSystem.ServiceDefaults;
using MudBlazor;
using MudBlazor.Services;
using MudExtensions.Services;

namespace eSecurity.Client.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.AddServiceDefaults();
            builder.AddValidation<IAssemblyMarker>();
            builder.AddVersioning();
            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                var gatewayUrl = builder.Configuration.GetValue<string>("PROXY_HTTPS");
                client.BaseAddress = new Uri(gatewayUrl ?? throw new NullReferenceException("Gateway URI is empty."));
            });
            
            builder.Services.Configure<BackendOptions>(builder.Configuration.GetSection("Backend"));
            builder.Services.AddSecurity();
            builder.Services.AddConfirmation();
            builder.Services.AddJs();
            builder.Services.AddState();
            builder.Services.AddGateway();
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