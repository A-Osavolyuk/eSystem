using eShop.Application.Documentation.Transformers;
using eShop.SmsSender.Api.Interfaces;

namespace eShop.SmsSender.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddLogging();
        builder.AddServiceDefaults();
        builder.AddJwtAuthentication();
        builder.AddVersioning();
        builder.AddValidation();
        builder.AddDependencyInjection();
        builder.AddMessageBus();
        builder.AddMediatR();
        builder.AddCors();
        builder.AddExceptionHandler();
        builder.AddDocumentation();
        builder.Services.AddControllers();

        return builder;
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
        });
    }
    
    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISmsService, SmsService>();
        builder.Services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
            new AmazonSimpleNotificationServiceClient(RegionEndpoint.EUNorth1));
    }

    private static void AddMessageBus(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var uri = builder.Configuration["Configuration:Services:MessageBus:RabbitMq:HostUri"]!;
                var username = builder.Configuration["Configuration:Services:MessageBus:RabbitMq:UserName"]!;
                var password = builder.Configuration["Configuration:Services:MessageBus:RabbitMq:Password"]!;

                cfg.Host(new Uri(uri), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ReceiveEndpoint("sms:verify-phone-number", e => e.ConfigureConsumer<VerifyPhoneNumberConsumer>(context));
                cfg.ReceiveEndpoint("sms:change-phone-number", e => e.ConfigureConsumer<ChangePhoneNumberConsumer>(context));
                cfg.ReceiveEndpoint("sms:two-factor-token", e => e.ConfigureConsumer<TwoFactorTokenConsumer>(context));
            });

            x.AddConsumer<VerifyPhoneNumberConsumer>();
            x.AddConsumer<ChangePhoneNumberConsumer>();
            x.AddConsumer<TwoFactorTokenConsumer>();
        });
    }
}