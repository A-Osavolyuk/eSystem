namespace eShop.Domain.Responses.Auth;

public class VerifyPasskeySignInResponse
{
    public Guid UserId { get; set; }
    public bool IsLockedOut { get; set; }
}