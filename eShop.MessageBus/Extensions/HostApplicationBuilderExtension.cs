using eShop.Application.Extensions;
using eShop.Domain.Common.Messaging;
using eShop.Domain.Enums;
using eShop.Domain.Messages.Email;
using eShop.Domain.Messages.Sms;
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
        builder.AddMessageBus(cfg =>
        {
            cfg.AddQueue<ChangeEmailMessage>("email-change", SenderType.Email);
            cfg.AddQueue<VerifyEmailMessage>("email-verification", SenderType.Email);
            cfg.AddQueue<EmailVerifiedMessage>("email-verified", SenderType.Email);
            cfg.AddQueue<NewEmailVerificationMessage>("new-email-verified", SenderType.Email);
            cfg.AddQueue<OAuthRegistrationEmailMessage>("oauth-registration", SenderType.Email);
            cfg.AddQueue<ResetPasswordEmailMessage>("password-reset", SenderType.Email);
            cfg.AddQueue<TwoFactorTokenEmailMessage>("two-factor-token", SenderType.Email);
            
            cfg.AddQueue<ChangePhoneNumberSmsMessage>("phone-number-change", SenderType.Sms);
            cfg.AddQueue<VerifyPhoneNumberSmsMessage>("phone-number-verification", SenderType.Sms);
            cfg.AddQueue<TwoFactorTokenSmsMessage>("two-factor-token", SenderType.Sms);
        });
        
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