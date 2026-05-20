using System.Text.Json.Serialization;
using eSecurity.Server.Common.Proxy;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Authentication;
using eSecurity.Server.Security.Cors;
using eSecurity.Server.Common.Cache;
using eSystem.Core.Gateway;
using eSystem.Core.Server.Bff;
using eSystem.ServiceDefaults;

namespace eSecurity.Server.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddServices(this IHostApplicationBuilder builder)
    {
        builder.AddRedisClient("redis");
        builder.AddSqlDb("e-security-bff-db");
        builder.AddProxy();
        builder.AddCors();
        builder.AddAuthentication();
        builder.AddBff();
        builder.AddServiceDefaults();

        builder.Services.AddHttpClient();
        builder.Services.AddGateway();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDataProtection();
        builder.Services.AddSingleton<ICache, RedisCache>();

        builder.Services.AddControllers()
            .AddJsonOptions(cfg =>
            {
                cfg.JsonSerializerOptions.WriteIndented = true;
                cfg.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
    }
}