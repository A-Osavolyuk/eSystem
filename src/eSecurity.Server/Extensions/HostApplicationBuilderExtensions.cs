using System.Text.Json.Serialization;
using eSecurity.Server.Common.Messaging;
using eSecurity.Server.Common.Storage;
using eSecurity.Server.Security;
using eSecurity.Server.Conventions;
using eSecurity.Server.Data;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Logging;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Http.Errors;
using eSystem.Core.Validation;

namespace eSecurity.Server.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddServices()
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

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services
                .AddControllers(options =>
                {
                    options.Conventions.Add(new RoutePrefixConvention("api"));
                })
                .AddJsonOptions(cfg =>
                {
                    cfg.JsonSerializerOptions.WriteIndented = true;
                    cfg.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>());
        }
    }
}