using eShop.Comments.Api.Services;
using eShop.Domain.Interfaces.API;

namespace eShop.Comments.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddLogging();
        builder.AddServiceDefaults();
        builder.AddJwtAuthentication();
        builder.AddVersioning();
        builder.AddValidation();
        builder.AddDependencyInjection();
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
    
    private static void AddRedisCache(this IHostApplicationBuilder builder)
    {
        builder.AddRedisClient("redis");
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            x.AddOpenBehavior(typeof(TransactionBehaviour<,>), ServiceLifetime.Transient);
        });
    }

    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICacheService, CacheService>();
    }

    private static void AddMessageBus(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("rabbit-mq");
                cfg.Host(connectionString);

                cfg.ReceiveEndpoint("product-deleted", e => e.ConfigureConsumer<ProductDeletedReceiver>(context));
            });

            x.AddConsumer<ProductDeletedReceiver>();
        });
    }
}