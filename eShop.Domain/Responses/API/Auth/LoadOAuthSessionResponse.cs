namespace eShop.Domain.Responses.API.Auth;

public class LoadOAuthSessionResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public OAuthSignType SignType { get; set; }
}