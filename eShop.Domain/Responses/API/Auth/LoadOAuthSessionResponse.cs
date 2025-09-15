namespace eShop.Domain.Responses.API.Auth;

public class LoadOAuthSessionResponse
{
    public string Provider { get; set; } = string.Empty;
    public OAuthSignType SignType { get; set; }
    public OAuthErrorType ErrorType { get; set; }
}