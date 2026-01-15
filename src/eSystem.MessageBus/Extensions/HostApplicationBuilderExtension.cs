using eSystem.Core.Http.Errors;
using eSystem.MessageBus.Consumers;
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
            builder.AddExceptionHandler();
            builder.AddServiceDefaults();
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