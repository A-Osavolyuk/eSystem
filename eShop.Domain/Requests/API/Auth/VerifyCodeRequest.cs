namespace eShop.Domain.Requests.API.Auth;

public class VerifyCodeRequest
{
    public Guid UserId { get; set; }
    public required string Code { get; set; }
    public CodeResource Resource  { get; set; }
    public CodeType Type  { get; set; }
    public SenderType Sender { get; set; }
}