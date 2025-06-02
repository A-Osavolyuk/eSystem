using eShop.Application.Extensions;
using eShop.Domain.Enums;
using eShop.Domain.Messages.Email;
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
        builder.AddMessageBus(cfg =>
        {
            cfg.AddQueue<ChangeEmailMessage>("email:email-change", SenderType.Email);
        });
        
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