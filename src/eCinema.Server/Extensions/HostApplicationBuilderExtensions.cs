using System.Text.Json.Serialization;
using eCinema.Server.Cache;
using eCinema.Server.Security.Authentication.Oidc;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Http.Errors;
using eSystem.Core.Security.Authentication.Oidc;
using eSystem.Core.Security.Authentication.Oidc.Client;
using eSystem.Core.Security.Authentication.Oidc.Constants;
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
            builder.AddRedisCache();
            
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddScoped<IOpenIdDiscoveryProvider, OpenIdDiscoveryProvider>();
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            });
            
            builder.Services.AddClient(cfg =>
            {
                cfg.ClientAudience = "eCinema";
                cfg.ClientId = "307268b0-005c-4ee4-a0e8-a93bd0010382";
                cfg.ClientSecret = "7fd5a079ecd90974a56532138e204ec0c42df875a06a0dedbe69797b609150c10162abed";
                cfg.CallbackUri = "https://localhost:6204/api/v1/connect/callback";
                cfg.PostLogoutRedirectUri = "https://localhost:6502/connect/logged-out";
                cfg.SupportedScopes =
                [
                    Scopes.OpenId,
                    Scopes.OfflineAccess,
                    Scopes.Email,
                    Scopes.Phone,
                    Scopes.Profile,
                    Scopes.Address
                ];
                cfg.SupportedPrompts =
                [
                    Prompts.Login,
                    Prompts.Consent
                ];
            });

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