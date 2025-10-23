using eSystem.Application.Common.Cache.Redis;
using eSystem.Application.Common.Documentation;
using eSystem.Application.Common.Errors;
using eSystem.Application.Common.Logging;
using eSystem.Application.Common.Versioning;
using eSystem.Application.Security.Authentication;
using eSystem.Application.Validation;
using eSystem.Comments.Api.Data;

namespace eSystem.Comments.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddLogging();
        builder.AddServiceDefaults();
        builder.AddJwtAuthentication();
        builder.AddVersioning();
        builder.AddValidation<IAssemblyMarker>();
        builder.AddMessageBus();
        builder.AddRedisCache();
        builder.AddMediatR();
        builder.AddExceptionHandler();
        builder.AddMsSqlDb();
        builder.AddDocumentation();
        builder.Services.AddControllers();
    }

    private static void AddMsSqlDb(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<AppDbContext>("comment-db");
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
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