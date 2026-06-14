using eSystem.Core.Server.Documentation;
using eSystem.Core.Server.Errors;
using eSystem.Core.Server.Mediator;
using eSystem.Core.Server.Versioning;
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
            builder.AddAzure();
            builder.AddDocumentation();
            builder.AddExceptionHandling<GlobalExceptionHandler>();
            
            builder.Services.AddMediator(cfg =>
            {
                cfg.AddRequestHandlersFromAssembly<IAssemblyMarker>();
            });
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