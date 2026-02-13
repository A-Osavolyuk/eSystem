using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Error;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Mediator;
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
            builder.AddRedisCache();
            builder.AddDocumentation();
            builder.AddExceptionHandling<GlobalExceptionHandler>();
            
            builder.Services.AddMediator<IAssemblyMarker>();
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