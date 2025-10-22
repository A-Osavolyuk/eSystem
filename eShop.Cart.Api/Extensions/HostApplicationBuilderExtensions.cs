using eShop.Application.Common.Cache.Redis;
using eShop.Application.Common.Documentation;
using eShop.Application.Common.Errors;
using eShop.Application.Common.Logging;
using eShop.Application.Common.Versioning;
using eShop.Application.Security.Authentication;
using eShop.Application.Validation;

namespace eShop.Cart.Api.Extensions;

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