using eSystem.Core.Common.Error;
using eSystem.MessageBus.Consumers;
using eSystem.MessageBus.Errors;
using eSystem.ServiceDefaults;
using MassTransit;

namespace eSystem.MessageBus.Extensions;

public static class HostApplicationBuilderExtension
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.AddMessageBus();
            builder.AddServiceDefaults();
            builder.AddExceptionHandling<GlobalExceptionHandler>();
        }

        private void AddMessageBus()
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
}