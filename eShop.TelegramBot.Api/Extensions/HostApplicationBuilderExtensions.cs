namespace eShop.TelegramBot.Api.Extensions;

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
        builder.AddExceptionHandler();
        builder.AddTelegramBot();
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
    }

    private static void AddTelegramBot(this IHostApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection("Configuration:Services:Bots:Telegram");
        builder.Services.Configure<BotOptions>(section);
        builder.Services.AddHttpClient("tgwebhook")
            .RemoveAllLoggers()
            .AddTypedClient<ITelegramBotClient>(client =>
                new TelegramBotClient(section.Get<BotOptions>()!.Token, client));
        builder.Services.AddSingleton<UpdateHandler>();
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
            });
        });
    }
}