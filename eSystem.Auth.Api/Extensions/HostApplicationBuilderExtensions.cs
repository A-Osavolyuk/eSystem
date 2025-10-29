using eSystem.Auth.Api.Messaging;
using eSystem.Auth.Api.Security;
using eSystem.Auth.Api.Storage.Session;
using eSystem.Core.Attributes;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Logging;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Data;
using eSystem.Core.Validation;

namespace eSystem.Auth.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddVersioning();
        builder.AddMessaging();
        builder.AddValidation<IAssemblyMarker>();
        builder.AddServiceDefaults();
        builder.AddSecurity();
        builder.AddServices<IAssemblyMarker>();
        builder.AddRedisCache();
        builder.AddMediatR();
        builder.AddMsSqlDb();
        builder.AddLogging();
        builder.AddExceptionHandler();
        builder.AddDocumentation();
        builder.AddSession();
        
        builder.Services.AddControllers()
            .AddJsonOptions(cfg => cfg.JsonSerializerOptions.WriteIndented = true);
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDistributedMemoryCache();
    }

    private static void AddSession(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISessionStorage, SessionStorage>();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(5);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>(); });
    }

    private static void AddMsSqlDb(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<AuthDbContext>("auth-db",
            configureDbContextOptions: cfg =>
            {
                cfg.UseAsyncSeeding(async (ctx, _, ct) =>
                {
                    await ctx.SeedAsync<IAssemblyMarker>(ct);
                });
            });
    }
}