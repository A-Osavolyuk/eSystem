namespace eShop.Domain.Requests.API.Auth;

public class VerifyCodeRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public CodeType Type { get; set; }
    public SenderType Sender { get; set; }
}