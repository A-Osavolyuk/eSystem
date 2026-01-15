using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Http.Errors;
using eSystem.EmailSender.Api.Consumers;
using eSystem.EmailSender.Api.Services;

namespace eSystem.EmailSender.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddApiServices()
        {
            builder.AddServiceDefaults();
            builder.AddMessageBus();
            builder.AddExceptionHandler();
            builder.AddDependencyInjection();
            builder.AddDocumentation();
            builder.AddRedisCache();
        }

        private void AddDependencyInjection()
        {
            builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("SMTP"));
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddOptions();
        }

        private void AddMessageBus()
        {
            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var connectionString = builder.Configuration.GetConnectionString("rabbit-mq");
                    cfg.Host(connectionString);
                
                    cfg.ReceiveEndpoint("email-message", (e) => e.ConfigureConsumer<EmailConsumer>(context));
                });

                x.AddConsumer<EmailConsumer>();
            });
        }
    }
}