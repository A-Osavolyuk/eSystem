using eAccount.Common.Http;
using eAccount.Common.JS;
using eAccount.Common.State;
using eAccount.Common.Storage;
using eAccount.Security;
using eAccount.Validation;
using eSystem.Core.Common.Network.Gateway;
using MudBlazor.Services;
using MudExtensions.Services;

namespace eAccount.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Configuration:Logging"));
        
        builder.AddServiceDefaults();
        builder.AddValidation();
        builder.AddState();
        builder.AddJs();
        builder.AddHttp();
        builder.AddSecurity();
        builder.AddStorage();
        
        builder.Services.AddControllers();
        builder.Services.AddGateway();
        builder.Services.AddControllers();
        builder.Services.AddLocalization(cfg => cfg.ResourcesPath = "Resources");
        builder.Services.AddMudExtensions();
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