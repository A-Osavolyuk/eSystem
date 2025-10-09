namespace eShop.Domain.Responses.Auth;

public class PasskeySignInResponse
{
    public Guid UserId { get; set; }
    public bool IsLockedOut { get; set; }
}