using eShop.Domain.Common.Api;
using eShop.Domain.Requests.Api.Telegram;
using Response = eShop.Domain.Common.Api.Response;

namespace eShop.TelegramBot.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class TelegramController(
    IOptions<BotOptions> options,
    ITelegramBotClient bot,
    UpdateHandler updateHandler) : ControllerBase
{
    private readonly ITelegramBotClient bot = bot;
    private readonly UpdateHandler updateHandler = updateHandler;
    private readonly BotOptions options = options.Value;

    [EndpointSummary("Set webhook")]
    [EndpointDescription("Sets the webhook")]
    [ProducesResponseType(200)]
    [HttpGet("setWebhook")]
    public async ValueTask<IActionResult> SetWebHookAsync(CancellationToken ct)
    {
        var webhookUrl = options.WebhookUrl;
        await bot.SetWebhook(webhookUrl, allowedUpdates: [], secretToken: options.Secret, cancellationToken: ct);
        return Ok($"Webhook set to {webhookUrl}");
    }

    [EndpointSummary("Update")]
    [EndpointDescription("Handles any changes related with bot")]
    [ProducesResponseType(200)]
    [HttpPost("update")]
    public async ValueTask<IActionResult> UpdateAsync([FromBody] Update update, CancellationToken ct)
    {
        if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != options.Secret)
            return Forbid();
        try
        {
            await updateHandler.OnUpdateAsync(bot, update, ct);
        }
        catch (Exception exception)
        {
            await updateHandler.OnErrorAsync(bot, exception, HandleErrorSource.HandleUpdateError, ct);
        }

        return Ok();
    }

    [EndpointSummary("Send message")]
    [EndpointDescription("Sends a message via Telegram")]
    [ProducesResponseType(200)]
    [HttpPost("send-message")]
    public async ValueTask<ActionResult<Response>> SendAsync([FromBody] SendMessageRequest request)
    {
        await bot.SendMessage(chatId: new ChatId(request.ChatId), text: request.Message);

        return Ok(new ResponseBuilder().WithMessage("Message was successfully sent!").Build());
    }
}