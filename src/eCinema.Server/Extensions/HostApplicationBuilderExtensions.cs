using System.Text.Json.Serialization;
using eCinema.Server.Common.Api;
using eCinema.Server.Common.Cache;
using eCinema.Server.Common.Errors;
using eCinema.Server.Security;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Error;
using eSystem.Core.Common.Gateway;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Http;
using eSystem.ServiceDefaults;

namespace eCinema.Server.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.AddServiceDefaults();
            builder.AddDocumentation();
            builder.AddVersioning();
            builder.AddRedisCache();
            builder.AddExceptionHandling<GlobalExceptionHandler>();
            builder.AddSecurity();
            builder.AddApiClients();
            
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            });
            
            builder.Services.AddOpenApi();
            builder.Services.AddGateway();
            builder.Services.AddHttpContextAccessor();
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

        private void AddApiClients()
        {
            var gatewayUrl = builder.Configuration.GetValue<string>("PROXY_HTTPS") 
                             ?? throw new NullReferenceException("Gateway URI is empty.");
            
            builder.Services.AddHttpClient("OIDC", client =>
            {
                client.BaseAddress = new Uri(gatewayUrl);
            });
            
            builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                client.BaseAddress = new Uri(gatewayUrl);
            });
        }
    }
}