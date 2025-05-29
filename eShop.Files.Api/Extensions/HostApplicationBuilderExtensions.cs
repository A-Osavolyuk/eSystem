using eShop.Application.Extensions;
using eShop.Domain.Interfaces.API;
using eShop.Files.Api.Interfaces;
using eShop.ServiceDefaults;

namespace eShop.Files.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddLogging();
        builder.AddJwtAuthentication();
        builder.AddVersioning();
        builder.AddDependencyInjection();
        builder.AddServiceDefaults();
        builder.AddRedisCache();
        builder.AddMediatR();
        builder.AddAzure();
        builder.AddExceptionHandler();
        builder.AddDocumentation();
        builder.Services.AddControllers();
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(IAssemblyMarker));
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
        });
    }
    
    private static void AddRedisCache(this IHostApplicationBuilder builder)
    {
        builder.AddRedisClient("redis");
    }

    private static void AddAzure(this IHostApplicationBuilder builder)
    {
        builder.AddAzureBlobClient("blobs");
    }

    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IStoreService, StoreService>();
        builder.Services.AddScoped<ICacheService, CacheService>();
    }
}