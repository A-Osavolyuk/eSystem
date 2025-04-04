namespace eShop.Domain.Abstraction.Messaging;

public abstract class SmsMessage : Message
{
    public string PhoneNumber { get; set; } = string.Empty;
}