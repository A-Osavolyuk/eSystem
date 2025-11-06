using System.Text.Json.Serialization;
using eSecurity.Common.JS;
using eSecurity.Common.State;
using eSecurity.Common.Storage;
using eSecurity.Messaging;
using eSecurity.Security;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Logging;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Validation;
using MudBlazor;
using MudBlazor.Services;
using MudExtensions.Services;

namespace eSecurity.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddServices(this IHostApplicationBuilder builder)
    {
        builder.AddVersioning();
        builder.AddMessaging();
        builder.AddValidation<IAssemblyMarker>();
        builder.AddServiceDefaults();
        builder.AddSecurity();
        builder.AddRedisCache();
        builder.AddMsSqlDb();
        builder.AddLogging();
        builder.AddExceptionHandler();
        builder.AddDocumentation();
        builder.AddStorage();
        builder.AddState();
        builder.AddJs();
        
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

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDistributedMemoryCache();
        builder.Services
            .AddControllers()
            .AddJsonOptions(cfg =>
            {
                cfg.JsonSerializerOptions.WriteIndented = true;
                cfg.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>());
    }
}