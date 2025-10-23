using eSystem.Application.Common.Cache.Redis;
using eSystem.Application.Common.Documentation;
using eSystem.Application.Common.Errors;
using eSystem.Application.Common.Logging;
using eSystem.Application.Common.Versioning;
using eSystem.Application.Security.Authentication;
using eSystem.Application.Validation;

namespace eSystem.Cart.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddLogging();
        builder.AddJwtAuthentication();
        builder.AddVersioning();
        builder.AddMongoDb();
        builder.AddMessageBus();
        builder.AddValidation<IAssemblyMarker>();
        builder.AddServiceDefaults();
        builder.AddRedisCache();
        builder.AddMediatR();
        builder.AddExceptionHandler();
        builder.AddDocumentation();
        builder.Services.AddControllers();
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
        });
    }
    
    private static void AddMongoDb(this IHostApplicationBuilder builder)
    {
        builder.AddMongoDBClient("cart-db");
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