using eSystem.Application.Common.Cache.Redis;
using eSystem.Application.Common.Documentation;
using eSystem.Application.Common.Errors;
using eSystem.Application.Common.Logging;
using eSystem.EmailSender.Api.Consumers;
using eSystem.EmailSender.Api.Interfaces;
using eSystem.EmailSender.Api.Options;
using eSystem.EmailSender.Api.Services;

namespace eSystem.EmailSender.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.AddLogging();
        builder.AddMessageBus();
        builder.AddExceptionHandler();
        builder.AddDependencyInjection();
        builder.AddDocumentation();
        builder.AddRedisCache();
    }

    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Configuration:Services:SMTP"));
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddOptions();
    }

    private static void AddMessageBus(this IHostApplicationBuilder builder)
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