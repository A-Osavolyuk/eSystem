using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Common.Responses;

public class LoadOAuthSessionResponse
{
    public Guid UserId { get; set; }
    public string LinkedAccount { get; set; } = string.Empty;
    public OAuthSignType SignType { get; set; }
    public OAuthErrorType ErrorType { get; set; }
}