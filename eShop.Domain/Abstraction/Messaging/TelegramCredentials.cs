namespace eShop.Domain.Abstraction.Messaging;

public class TelegramCredentials : MessageCredentials
{
    public long ChatId { get; set; }
}