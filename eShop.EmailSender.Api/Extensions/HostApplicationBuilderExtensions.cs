using eShop.Application.Extensions;
using eShop.EmailSender.Api.Consumers;
using eShop.EmailSender.Api.Services;

namespace eShop.EmailSender.Api.Extensions;

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
    
    private static void AddRedisCache(this IHostApplicationBuilder builder)
    {
        builder.AddRedisClient("redis");
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

                cfg.ReceiveEndpoint("email:account-recovery", e => e.ConfigureConsumer<AccountRecoveryConsumer>(context));
                cfg.ReceiveEndpoint("email:password-reset", e => e.ConfigureConsumer<ResetPasswordConsumer>(context));
                cfg.ReceiveEndpoint("email:email-change", e => e.ConfigureConsumer<ChangeEmailConsumer>(context));
                cfg.ReceiveEndpoint("email:email-verification", e => e.ConfigureConsumer<VerifyEmailConsumer>(context));
                cfg.ReceiveEndpoint("email:email-verified", e => e.ConfigureConsumer<EmailVerifiedConsumer>(context));
                cfg.ReceiveEndpoint("email:two-factor-token", e => e.ConfigureConsumer<TwoFactorTokenConsumer>(context));
                cfg.ReceiveEndpoint("email:external-provider-registration", e => e.ConfigureConsumer<ExternalLoginConsumer>(context));
            });

            x.AddConsumer<AccountRecoveryConsumer>();
            x.AddConsumer<VerifyEmailConsumer>();
            x.AddConsumer<ResetPasswordConsumer>();
            x.AddConsumer<ChangeEmailConsumer>();
            x.AddConsumer<EmailVerifiedConsumer>();
            x.AddConsumer<TwoFactorTokenConsumer>();
            x.AddConsumer<ExternalLoginConsumer>();
        });
    }
}