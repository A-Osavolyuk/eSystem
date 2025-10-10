namespace eShop.Domain.Responses.Auth;

public class AuthenticatorSignInResponse
{
    public Guid UserId { get; set; }
    
    public int FailedLoginAttempts { get; set; }
    public int MaxFailedLoginAttempts { get; set; }

    public bool IsLockedOut { get; set; }
    public LockoutType Type { get; set; }
}