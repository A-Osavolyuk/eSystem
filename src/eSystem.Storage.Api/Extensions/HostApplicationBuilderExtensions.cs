using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Error;
using eSystem.Core.Common.Versioning;
using eSystem.ServiceDefaults;
using eSystem.Storage.Api.Errors;
using eSystem.Storage.Api.Interfaces;

namespace eSystem.Storage.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddApiServices()
        {
            builder.AddVersioning();
            builder.AddDependencyInjection();
            builder.AddServiceDefaults();
            builder.AddRedisCache();
            builder.AddMediatR();
            builder.AddAzure();
            builder.AddDocumentation();
            builder.AddExceptionHandling<GlobalExceptionHandler>();
            
            builder.Services.AddProblemDetails();
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