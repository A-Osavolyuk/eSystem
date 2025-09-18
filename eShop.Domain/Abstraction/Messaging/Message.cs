namespace eShop.Domain.Abstraction.Messaging;

public abstract class Message
{
    public required Dictionary<string, string> Credentials { get; set; } = [];
    public required Dictionary<string, string> Payload { get; set; } = [];
    
    public abstract string Build();
    public abstract void Initialize(Dictionary<string, string> payload);
}