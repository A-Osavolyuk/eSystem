using eSystem.Application.Common.Cache.Redis;
using eSystem.Application.Common.Documentation;
using eSystem.Application.Common.Errors;
using eSystem.Application.Common.Logging;
using eSystem.Application.Common.Versioning;
using eSystem.Application.Data;
using eSystem.Application.Validation;
using eSystem.Auth.Api.Security;
using eSystem.Auth.Api.Services;
using MassTransit;

namespace eSystem.Auth.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddVersioning();
        builder.AddMessageBus();
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

    private static void AddMessageBus(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("rabbit-mq");
                cfg.Host(connectionString);
            });
        });
    }
}