using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.DTOs;

public sealed class AuthenticationSessionDto
{
    public Guid? SessionId { get; set; }
    
    public string? IdentityProvider { get; set; }
    public OAuthFlow? OAuthFlow { get; set; }
    
    public bool IsCompleted { get; set; }
    public string? NextMethod { get; set; }

    public string[]? AllowedMfaMethods { get; set; }

}