namespace eShop.Domain.Requests.Auth;

public class VerifyAuthenticatorCodeRequest
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
    public required CodeResource Resource { get; set; }
    public required CodeType Type { get; set; }
}