using eShop.SmsSender.Api.Interfaces;
using FluentValidation;

namespace eShop.SmsSender.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddLogging();
        builder.AddServiceDefaults();
        builder.AddJwtAuthentication();
        builder.AddVersioning();
        builder.AddValidation();
        builder.AddDependencyInjection();
        builder.AddMessageBus();
        builder.AddMediatR();
        builder.AddRedisCache();
        builder.AddExceptionHandler();
        builder.AddDocumentation();
        builder.Services.AddControllers();

        return builder;
    }
    
    private static void AddRedisCache(this IHostApplicationBuilder builder)
    {
        builder.AddRedisClient("redis");
    }
    
    private static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
        });
    }
    
    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISmsService, SmsService>();
        builder.Services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
            new AmazonSimpleNotificationServiceClient(RegionEndpoint.EUNorth1));
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