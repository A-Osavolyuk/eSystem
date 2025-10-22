using eShop.Domain.Security.Authorization.OAuth;

namespace eShop.Domain.Responses.Auth;

public class LoadOAuthSessionResponse
{
    public Guid UserId { get; set; }
    public string LinkedAccount { get; set; } = string.Empty;
    public OAuthSignType SignType { get; set; }
    public OAuthErrorType ErrorType { get; set; }
}