using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.Responses;

public class LoadOAuthSessionResponse
{
    public Guid UserId { get; set; }
    public string LinkedAccount { get; set; } = string.Empty;
    public OAuthSignType SignType { get; set; }
    public OAuthErrorType ErrorType { get; set; }
}