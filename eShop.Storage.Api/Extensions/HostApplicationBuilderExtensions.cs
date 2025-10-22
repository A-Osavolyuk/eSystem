using eShop.Application.Common.Cache.Redis;
using eShop.Application.Common.Documentation;
using eShop.Application.Common.Errors;
using eShop.Application.Common.Logging;
using eShop.Application.Common.Versioning;
using eShop.Application.Security.Authentication;
using eShop.ServiceDefaults;
using eShop.Storage.Api.Interfaces;

namespace eShop.Storage.Api.Extensions;

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

    private static void AddAzure(this IHostApplicationBuilder builder)
    {
        builder.AddAzureBlobServiceClient("blobs");
    }

    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IStorageManager, StorageManager>();
    }
}