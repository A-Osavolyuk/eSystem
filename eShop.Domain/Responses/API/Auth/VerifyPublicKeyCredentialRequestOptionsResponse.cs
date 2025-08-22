namespace eShop.Domain.Responses.API.Auth;

public class VerifyPublicKeyCredentialRequestOptionsResponse
{
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public bool IsLockedOut { get; set; }
}