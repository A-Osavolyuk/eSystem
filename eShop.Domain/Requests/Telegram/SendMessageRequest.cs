namespace eShop.Domain.Requests.Telegram;

public class SendMessageRequest
{
    public long ChatId { get; set; }
    public string Message { get; set; } = string.Empty;
}