using eSystem.Application.Common.Cache.Redis;
using eSystem.Application.Common.Documentation;
using eSystem.Application.Common.Errors;
using eSystem.Application.Common.Logging;
using eSystem.Application.Common.Versioning;
using eSystem.Application.Security.Authentication;
using eSystem.ServiceDefaults;
using eSystem.Storage.Api.Interfaces;
using eSystem.Storage.Api.Services;

namespace eSystem.Storage.Api.Extensions;

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