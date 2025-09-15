namespace eShop.Domain.Responses.API.Auth;

public class VerifyPasskeySignInResponse
{
    public Guid UserId { get; set; }
    public bool IsLockedOut { get; set; }
}