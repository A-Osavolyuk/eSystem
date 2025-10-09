namespace eShop.Domain.Requests.Auth;

public record AuthenticatorSignInRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}