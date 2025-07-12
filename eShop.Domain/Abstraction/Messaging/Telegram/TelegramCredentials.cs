namespace eShop.Domain.Abstraction.Messaging.Telegram;

public class TelegramCredentials : MessageCredentials
{
    public long ChatId { get; set; }
}