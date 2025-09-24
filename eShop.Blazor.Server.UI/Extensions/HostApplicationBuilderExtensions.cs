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
    }
}