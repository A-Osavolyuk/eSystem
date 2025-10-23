namespace eSystem.Domain.Common.Messaging;

public class MessageRequest
{
    public Guid Id { get; set; }
    public SenderType Type { get; set; }
    public required string Body { get; set; }
    public required Dictionary<string, string> Credentials { get; set; }
}