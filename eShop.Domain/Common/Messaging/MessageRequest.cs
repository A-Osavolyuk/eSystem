namespace eShop.Domain.Common.Messaging;

public class MessageRequest
{
    public MessageType Type { get; set; }
    public required string Queue { get; set; }
    public required Dictionary<string, string> Payload { get; set; }
    public required Dictionary<string, string> Credentials { get; set; }
}