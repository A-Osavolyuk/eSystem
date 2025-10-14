namespace eShop.Domain.Requests.Auth;

public class VerifyRecoveryCodeRequest
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
}