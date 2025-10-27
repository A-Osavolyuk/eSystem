using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Logging;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Security.Authentication;
using eSystem.ServiceDefaults;
using eSystem.Storage.Api.Interfaces;

namespace eSystem.Storage.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddLogging();
        builder.AddAuthentication();
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