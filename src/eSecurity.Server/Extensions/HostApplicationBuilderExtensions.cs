using System.Text.Json.Serialization;
using eSecurity.Server.Common.Errors;
using eSecurity.Server.Common.Mapping;
using eSecurity.Server.Common.Messaging;
using eSecurity.Server.Common.Middlewares;
using eSecurity.Server.Common.Storage;
using eSecurity.Server.Security;
using eSecurity.Server.Conventions;
using eSecurity.Server.Data;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Error;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Mediator;
using eSystem.Core.Validation;

namespace eSecurity.Server.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.AddMapping();
            builder.AddVersioning();
            builder.AddMessaging();
            builder.AddValidation<IAssemblyMarker>();
            builder.AddServiceDefaults();
            builder.AddSecurity();
            builder.AddRedisCache();
            builder.AddDatabase();
            builder.AddDocumentation();
            builder.AddStorage();
            builder.AddExceptionHandling<GlobalExceptionHandler>();

            builder.Services.AddTransient<RequestBufferingMiddleware>();
            builder.Services.AddMediator<IAssemblyMarker>();
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
        }
    }
}