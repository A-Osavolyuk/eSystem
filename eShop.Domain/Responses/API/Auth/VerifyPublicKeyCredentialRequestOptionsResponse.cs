namespace eShop.Domain.Responses.API.Auth;

public class VerifyPublicKeyCredentialRequestOptionsResponse
{
    public Guid UserId { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}