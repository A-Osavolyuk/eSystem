using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Common.Messaging.Sms;
using eSecurity.Idp.Common.Messaging.Sms.Builders;
using MassTransit;

namespace eSecurity.Idp.Common.Messaging;

public static class MessagingExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddMessaging()
        {
            builder.Services.AddMessageBus();
            
            builder.Services.AddScoped<ISmsService, SmsService>();
            builder.Services.AddScoped<ISmsBuilderProvider, SmsBuilderProvider>();
            builder.Services.AddSmsBuilder<CodeVerificationSmsContext, CodeVerificationSmsBuilder>();
            
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IEmailBuilderProvider, EmailBuilderProvider>();
            builder.Services.AddEmailBuilder<EmailVerificationContext, EmailVerificationBuilder>();
            builder.Services.AddEmailBuilder<CodeVerificationEmailContext, CodeVerificationEmailBuilder>();
            builder.Services.AddEmailBuilder<OAuthSignedUpEmailContext, OAuthSignedUpEmailBuilder>();
        }
    }

    extension(IServiceCollection services)
    {
        private void AddEmailBuilder<TContext, TBuilder>()
            where TContext : EmailContext
            where TBuilder : class, IEmailBuilder<TContext>
        {
            services.AddTransient<IEmailBuilder<TContext>, TBuilder>();
        }
        
        private void AddSmsBuilder<TContext, TBuilder>()
            where TContext : SmsContext
            where TBuilder : class, ISmsBuilder<TContext>
        {
            services.AddTransient<ISmsBuilder<TContext>, TBuilder>();
        }

        private void AddMessageBus()
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
}