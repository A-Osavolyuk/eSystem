namespace eShop.Domain.Abstraction.Messaging;

public abstract class Message
{
    public required Dictionary<string, string> Credentials { get; set; } = [];
    
    public abstract string Build();
}