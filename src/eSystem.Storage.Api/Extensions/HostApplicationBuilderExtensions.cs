using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Error;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Mediator;
using eSystem.ServiceDefaults;
using eSystem.Storage.Api.Errors;

namespace eSystem.Storage.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddApiServices()
        {
            builder.AddVersioning();
            builder.AddServiceDefaults();
            builder.AddRedisCache();
            builder.AddAzure();
            builder.AddDocumentation();
            builder.AddExceptionHandling<GlobalExceptionHandler>();
            
            builder.Services.AddMediator<IAssemblyMarker>();
            builder.Services.AddProblemDetails();
            builder.Services.AddControllers();
            builder.Services.AddValidatorsFromAssemblyContaining(typeof(IAssemblyMarker));
        }

        private void AddAzure()
        {
            builder.AddAzureBlobServiceClient("blobs");
        }
    }
}