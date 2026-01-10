namespace eSystem.EmailSender.Api.Requests;

public class SendMessageRequest
{
    public string HtmlBody { get; set; } = string.Empty;
    public MessageOptions Options { get; set; } = null!;
}