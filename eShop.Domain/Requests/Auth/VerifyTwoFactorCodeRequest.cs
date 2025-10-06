namespace eShop.Domain.Requests.Auth;

public class VerifyTwoFactorCodeRequest
{
    public Guid UserId { get; set; }
    public required string Code { get; set; }
    public required string Provider { get; set; }
}