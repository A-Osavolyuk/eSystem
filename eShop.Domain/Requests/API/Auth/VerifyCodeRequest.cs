namespace eShop.Domain.Requests.API.Auth;

public class VerifyCodeRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public Verification CodeType { get; set; }
}