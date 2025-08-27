namespace eShop.Domain.Requests.API.Auth;

public class SendCodeRequest
{
    public Guid UserId { get; set; }
    public SenderType Sender { get; set; }
    public CodeType Type { get; set; }
    public CodeResource Resource { get; set; }

    public Dictionary<string, string> Payload { get; set; } = [];
}