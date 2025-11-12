using MassTransit;

namespace eSecurity.Server.Common.Messaging;

public static class MessagingExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddMessaging()
        {
            builder.Services.AddMessageBus();
            builder.Services.AddScoped<IMessageService, MessageService>();
        }
    }
    
    private static void AddMessageBus(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = configuration.GetConnectionString("rabbit-mq");
                cfg.Host(connectionString);
            });
        });
    }
}