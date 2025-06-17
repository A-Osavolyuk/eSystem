using Interfaces_IUpdateHandler = eShop.Telegram.Bot.Interfaces.IUpdateHandler;
using IUpdateHandler = eShop.Telegram.Bot.Interfaces.IUpdateHandler;

namespace eShop.Telegram.Bot.Services;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger) : Interfaces_IUpdateHandler
{
    private readonly ITelegramBotClient bot = bot;
    private readonly ILogger<UpdateHandler> logger = logger;

    public Task OnUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.CompletedTask;
    }

    public Task OnErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling error: {exception}", exception);

        return Task.CompletedTask;
    }
}