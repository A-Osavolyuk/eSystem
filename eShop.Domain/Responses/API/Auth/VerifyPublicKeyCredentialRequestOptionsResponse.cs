namespace eShop.Domain.Responses.API.Auth;

public class VerifyPublicKeyCredentialRequestOptionsResponse
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsLockedOut { get; set; }
}