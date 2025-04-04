namespace eShop.Domain.Requests.Api.Auth;

public class VerifyCodeRequest
{
    public string Code { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public VerificationCodeType CodeType { get; set; }
}