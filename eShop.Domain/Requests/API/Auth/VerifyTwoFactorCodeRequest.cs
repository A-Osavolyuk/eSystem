namespace eShop.Domain.Requests.API.Auth;

public class VerifyTwoFactorCodeRequest
{
    public Guid UserId { get; set; }
    public required string Code { get; set; }
    public required string Provider { get; set; }
}