namespace eShop.Domain.Abstraction.Messaging;

public abstract class EmailMessage : Message
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}