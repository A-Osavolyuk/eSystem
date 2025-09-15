namespace eShop.Domain.Responses.API.Auth;

public class VerifyPublicKeyCredentialRequestOptionsResponse
{
    public Guid UserId { get; set; }
    public bool IsLockedOut { get; set; }
}