using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.Responses;

public class LoadOAuthSessionResponse
{
    public required Guid UserId { get; set; }
    public required string Provider { get; set; }
    public required OAuthFlow Flow { get; set; }
}