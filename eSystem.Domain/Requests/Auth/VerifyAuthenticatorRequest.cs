namespace eSystem.Domain.Requests.Auth;

public class VerifyAuthenticatorRequest
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
    public required string Secret { get; set; }
}