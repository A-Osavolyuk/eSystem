using eSystem.Core.Messaging;

namespace eSystem.Core.Server.Messaging;

public class MessageRequest
{
    public SenderType Type { get; set; }
    public required string Body { get; set; }
    public required Dictionary<string, string> Credentials { get; set; }
}