using System.Text.Json;
using System.Text.Json.Serialization;
using eSecurity.Idp.Common.Errors;
using eSecurity.Idp.Conventions;
using eSecurity.Idp.Common.Binding;
using eSecurity.Idp.Common.Mapping;
using eSecurity.Idp.Common.Messaging;
using eSecurity.Idp.Common.Storage;
using eSecurity.Idp.Security;
using eSecurity.Idp.Data;
using eSecurity.Idp.Middlewares;
using eSystem.Core.Server.Documentation;
using eSystem.Core.Server.Errors;
using eSystem.Core.Server.Versioning;
using eSystem.Core.Validation;

namespace eSecurity.Idp.Extensions;

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
            builder.AddDatabase();
            builder.AddDocumentation();
            builder.AddStorage();
            builder.AddExceptionHandling<GlobalExceptionHandler>();

            builder.Services.AddDataBinding();
            builder.Services.AddTransient<RequestBufferingMiddleware>();
            builder.Services.AddMediator(cfg =>
            {
                cfg.AddRequestHandlersFromAssembly<IAssemblyMarker>();
            });
            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.WriteIndented = true;
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
            });
            
            builder.Services
                .AddControllers(options =>
                {
                    options.Conventions.Add(new RoutePrefixConvention("api"));
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
                });
        }
    }
}