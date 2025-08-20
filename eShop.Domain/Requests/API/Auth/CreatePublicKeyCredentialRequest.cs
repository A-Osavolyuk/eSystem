namespace eShop.Domain.Requests.API.Auth;

public class CreatePublicKeyCredentialRequest
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}