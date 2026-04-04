using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Core.Common.DTOs;

public sealed class AuthenticationSessionDto
{
    [JsonPropertyName("sid")]
    public Guid? SessionId { get; set; }
    
    [JsonPropertyName("identity_provider")]
    public string? IdentityProvider { get; set; }
    
    [JsonPropertyName("oauth_flow")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<OAuthFlow>))]
    public OAuthFlow? OAuthFlow { get; set; }
    
    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }
    
    [JsonPropertyName("next_method")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<AuthenticationMethod>))]
    public AuthenticationMethod? NextMethod { get; set; }
    
    [JsonPropertyName("allowed_mfa_methods")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<AuthenticationMethod>))]
    public IEnumerable<AuthenticationMethod> AllowedMfaMethods { get; set; } = [];

}