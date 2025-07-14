namespace eShop.Domain.Requests.API.Auth;

public class ResendCodeRequest
{
    public Guid UserId { get; set; }
    public SenderType Sender { get; set; }
    public CodeType CodeType { get; set; }
    public CodeResource CodeResource { get; set; }

    public Dictionary<string, string> Payload { get; set; } = [];
}