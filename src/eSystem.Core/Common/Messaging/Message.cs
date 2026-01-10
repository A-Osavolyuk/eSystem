namespace eSystem.Core.Common.Messaging;

public abstract class Message
{
    public Dictionary<string, string> Credentials { get; set; } = [];
    public Dictionary<string, string> Payload { get; set; } = [];
    
    public abstract string Build();
    public abstract void Initialize(Dictionary<string, string> payload);
}