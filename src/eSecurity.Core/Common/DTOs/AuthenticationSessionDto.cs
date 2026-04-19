using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Core.Common.DTOs;

public sealed class AuthenticationSessionDto
{
    [JsonPropertyName("sid")]
    public Guid? SessionId { get; set; }

    [JsonPropertyName("session_cookie")]
    public string? SessionCookie { get; set; }
    
    [JsonPropertyName("identity_provider")]
    public string? IdentityProvider { get; set; }
    
    [JsonPropertyName("oauth_flow")]
    public OAuthFlow? OAuthFlow { get; set; }
    
    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }

    [JsonPropertyName("require_mfa")]
    public bool RequireMfa { get; set; }
    
    [JsonPropertyName("next_method")]
    public AuthenticationMethodReference? NextMethod { get; set; }
    
    [JsonPropertyName("allowed_mfa_methods")]
    public IEnumerable<AuthenticationMethodReference> AllowedMfaMethods { get; set; } = [];

}