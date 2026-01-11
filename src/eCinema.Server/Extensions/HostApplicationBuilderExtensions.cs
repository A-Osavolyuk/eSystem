using System.Text.Json.Serialization;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Common.Versioning;
using eSystem.ServiceDefaults;

namespace eCinema.Server.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.AddServiceDefaults();
            builder.AddExceptionHandler();
            builder.AddDocumentation();
            builder.AddVersioning();

            builder.Services.AddOpenApi();
            builder.Services.AddGateway();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllers()
                .AddJsonOptions(cfg =>
                {
                    cfg.JsonSerializerOptions.WriteIndented = true;
                    cfg.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });;
        }
    }
}