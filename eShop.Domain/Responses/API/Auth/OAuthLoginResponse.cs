namespace eShop.Domain.Responses.API.Auth;

public class OAuthLoginResponse
{
    public AuthenticationProperties AuthenticationProperties { get; set; } = null!;
    public string Provider { get; set; } = string.Empty;
}