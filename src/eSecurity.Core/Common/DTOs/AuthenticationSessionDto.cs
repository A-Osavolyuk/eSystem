using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Core.Common.DTOs;

public sealed class AuthenticationSessionDto
{
    public Guid? SessionId { get; set; }
    
    public string? IdentityProvider { get; set; }
    public OAuthFlow? OAuthFlow { get; set; }
    
    public bool IsCompleted { get; set; }
    public AuthenticationMethod? NextMethod { get; set; }
    public IEnumerable<AuthenticationMethod> AllowedMfaMethods { get; set; } = [];

}