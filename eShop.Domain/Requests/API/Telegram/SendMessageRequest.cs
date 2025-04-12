namespace eShop.Domain.Requests.API.Telegram;

public class SendMessageRequest
{
    public long ChatId { get; set; }
    public string Message { get; set; } = string.Empty;
}