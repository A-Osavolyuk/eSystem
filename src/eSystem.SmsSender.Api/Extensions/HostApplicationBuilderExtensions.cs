using eSystem.Core.Server.Mediator;
using eSystem.Core.Server.Documentation;
using eSystem.Core.Server.Errors;
using eSystem.Core.Server.Versioning;
using eSystem.SmsSender.Api.Consumers;
using eSystem.SmsSender.Api.Errors;
using eSystem.SmsSender.Api.Interfaces;
using FluentValidation;

namespace eSystem.SmsSender.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddApiServices()
        {
            builder.AddServiceDefaults();
            builder.AddVersioning();
            builder.AddValidation();
            builder.AddDependencyInjection();
            builder.AddMessageBus();
            builder.AddDocumentation();
            builder.AddExceptionHandling<GlobalExceptionHandler>();
            
            builder.Services.AddMediator(cfg =>
            {
                cfg.FromAssembly<IAssemblyMarker>();
            });
            builder.Services.AddControllers();

            return builder;
        }

        private void AddValidation()
        {
            builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
        }

        private void AddDependencyInjection()
        {
            builder.Services.AddScoped<ISmsService, SmsService>();
            builder.Services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
                new AmazonSimpleNotificationServiceClient(RegionEndpoint.EUNorth1));
        }

        private void AddMessageBus()
        {
            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var connectionString = builder.Configuration.GetConnectionString("rabbit-mq");
                    cfg.Host(connectionString);
                
                    cfg.ReceiveEndpoint("sms-message", (e) => e.ConfigureConsumer<SmsConsumer>(context));
                });

                x.AddConsumer<SmsConsumer>();
            });
        }
    }
}