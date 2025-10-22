using eShop.Application.Common.Errors;
using eShop.Application.Common.Logging;
using eShop.MessageBus.Consumers;
using eShop.ServiceDefaults;
using MassTransit;

namespace eShop.MessageBus.Extensions;

public static class HostApplicationBuilderExtension
{
    public static void AddServices(this IHostApplicationBuilder builder)
    {
        builder.AddMessageBus();
        builder.AddExceptionHandler();
        builder.AddLogging();
        builder.AddServiceDefaults();
    }

    private static void AddMessageBus(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("rabbit-mq");
                cfg.Host(connectionString);

                cfg.ReceiveEndpoint("unified-message", e => e.ConfigureConsumer<MessageConsumer>(context));
            });

            x.AddConsumer<MessageConsumer>();
        });
    }
}