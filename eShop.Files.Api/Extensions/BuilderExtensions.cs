using eShop.Application.Extensions;
using eShop.Domain.Interfaces;
using eShop.Files.Api.Interfaces;
using eShop.Files.Api.Services;
using eShop.ServiceDefaults;
using Interfaces_IStoreService = eShop.Files.Api.Interfaces.IStoreService;

namespace eShop.Files.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Configuration:Logging"));

        builder.AddJwtAuthentication();
        builder.AddVersioning();
        builder.AddDependencyInjection();
        builder.AddServiceDefaults();
        builder.AddRedisCache();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            x.AddOpenBehavior(typeof(LoggingBehaviour<,>), ServiceLifetime.Transient);
        });

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(IAssemblyMarker));
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();
    }

    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IStoreService, StoreService>();
        builder.Services.AddScoped<ICacheService, CacheService>();
    }
}