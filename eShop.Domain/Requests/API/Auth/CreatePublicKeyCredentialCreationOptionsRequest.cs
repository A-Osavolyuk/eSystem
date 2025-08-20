namespace eShop.Domain.Requests.API.Auth;

public class CreatePublicKeyCredentialCreationOptionsRequest
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}