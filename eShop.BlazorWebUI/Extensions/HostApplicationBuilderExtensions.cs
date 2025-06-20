using eShop.BlazorWebUI.State;
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
        
        builder.Services.AddSingleton<UserState>();
    }

    private static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
    }
}