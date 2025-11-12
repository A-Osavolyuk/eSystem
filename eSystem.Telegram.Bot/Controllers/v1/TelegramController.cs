using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Telegram;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace eSystem.Telegram.Bot.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class TelegramController(
    IOptions<BotOptions> options,
    ITelegramBotClient bot,
    UpdateHandler updateHandler) : ControllerBase
{
    private readonly ITelegramBotClient _bot = bot;
    private readonly UpdateHandler _updateHandler = updateHandler;
    private readonly BotOptions _options = options.Value;

    [EndpointSummary("Set webhook")]
    [EndpointDescription("Sets the webhook")]
    [ProducesResponseType(200)]
    [HttpGet("setWebhook")]
    public async ValueTask<IActionResult> SetWebHookAsync(CancellationToken ct)
    {
        var webhookUrl = _options.WebhookUrl;
        await _bot.SetWebhook(webhookUrl, allowedUpdates: [], secretToken: _options.Secret, cancellationToken: ct);
        return Ok($"Webhook set to {webhookUrl}");
    }

    [EndpointSummary("Update")]
    [EndpointDescription("Handles any changes related with bot")]
    [ProducesResponseType(200)]
    [HttpPost("update")]
    public async ValueTask<IActionResult> UpdateAsync([FromBody] Update update, CancellationToken ct)
    {
        if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != _options.Secret)
            return Forbid();
        try
        {
            await _updateHandler.OnUpdateAsync(_bot, update, ct);
        }
        catch (Exception exception)
        {
            await _updateHandler.OnErrorAsync(_bot, exception, HandleErrorSource.HandleUpdateError, ct);
        }

        return Ok();
    }

    [EndpointSummary("Send message")]
    [EndpointDescription("Sends a message via Telegram")]
    [ProducesResponseType(200)]
    [HttpPost("send-message")]
    public async ValueTask<ActionResult<HttpResponse>> SendAsync([FromBody] SendMessageRequest request)
    {
        await _bot.SendMessage(chatId: new ChatId(request.ChatId), text: request.Message);

        return Ok(HttpResponseBuilder.Create().WithMessage("Message was successfully sent!").Build());
    }
}