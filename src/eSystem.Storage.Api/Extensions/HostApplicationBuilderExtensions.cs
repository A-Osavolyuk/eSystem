using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Logging;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Http.Errors;
using eSystem.ServiceDefaults;
using eSystem.Storage.Api.Interfaces;

namespace eSystem.Storage.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddApiServices()
        {
            builder.AddLogging();
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

        private void AddMediatR()
        {
            builder.Services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            });
        }

        private void AddAzure()
        {
            builder.AddAzureBlobServiceClient("blobs");
        }

        private void AddDependencyInjection()
        {
            builder.Services.AddScoped<IStorageManager, StorageManager>();
        }
    }
}