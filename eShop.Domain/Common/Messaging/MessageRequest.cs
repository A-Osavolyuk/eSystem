namespace eShop.Domain.Common.Messaging;

public class MessageRequest
{
    public SenderType Type { get; set; }
    public required string Queue { get; set; }
    public required Dictionary<string, string> Payload { get; set; }
    public required Dictionary<string, string> Credentials { get; set; }
}