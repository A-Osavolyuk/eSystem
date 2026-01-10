using IUpdateHandler = eSystem.Telegram.Bot.Interfaces.IUpdateHandler;

namespace eSystem.Telegram.Bot.Services;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IUpdateHandler
{
    private readonly ITelegramBotClient _bot = bot;
    private readonly ILogger<UpdateHandler> _logger = logger;

    public Task OnUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.CompletedTask;
    }

    public Task OnErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling error: {exception}", exception);

        return Task.CompletedTask;
    }
}