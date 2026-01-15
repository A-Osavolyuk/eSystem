using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Error;
using eSystem.Core.Common.Versioning;
using eSystem.Telegram.Bot.Errors;

namespace eSystem.Telegram.Bot.Extensions;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddApiServices()
        {
            builder.AddServiceDefaults();
            builder.AddVersioning();
            builder.AddMessageBus();
            builder.AddMediatR();
            builder.AddTelegramBot();
            builder.AddDocumentation();
            builder.AddRedisCache();
            builder.AddExceptionHandling<GlobalExceptionHandler>();
            
            builder.Services.AddControllers();
        }

        private void AddMediatR()
        {
            builder.Services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            });
        }

        private void AddTelegramBot()
        {
            var section = builder.Configuration.GetSection("Telegram");
            builder.Services.Configure<BotOptions>(section);
            builder.Services.AddHttpClient("tgwebhook")
                .RemoveAllLoggers()
                .AddTypedClient<ITelegramBotClient>(client =>
                    new TelegramBotClient(section.Get<BotOptions>()!.Token, client));
            builder.Services.AddSingleton<UpdateHandler>();
        }

        private void AddMessageBus()
        {
            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var connectionString = builder.Configuration.GetConnectionString("rabbit-mq");
                    cfg.Host(connectionString);
                });
            });
        }
    }
}