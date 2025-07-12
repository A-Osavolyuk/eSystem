namespace eShop.Domain.Abstraction.Messaging;

public abstract class Message
{
    public string Body { get; } = string.Empty;
    public Dictionary<string, string> Credentials { get; set; } = [];
    
    public abstract string Build();
}