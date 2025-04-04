namespace eShop.Domain.Requests.Api.Telegram;

public class SendMessageRequest
{
    public long ChatId { get; set; }
    public string Message { get; set; } = string.Empty;
}