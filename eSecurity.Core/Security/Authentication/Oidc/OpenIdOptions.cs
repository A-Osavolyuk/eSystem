using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Authentication.Oidc;

public class OpenIdOptions
{
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = string.Empty;
    

    [JsonPropertyName("authorization_endpoint")]
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("userinfo_endpoint")]
    public string UserinfoEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; } = string.Empty;
    
    [JsonPropertyName("end_session_endpoint")]
    public string EndSessionEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("introspection_endpoint")]
    public string IntrospectionEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("revocation_endpoint")]
    public string RevocationEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("device_authorization_endpoint")]
    public string DeviceAuthorizationEndpoint { get; set; } = string.Empty;
    
    
    [JsonPropertyName("grant_types_supported")]
    public string[] GrantTypesSupported { get; set; } = [];
    
    [JsonPropertyName("response_types_supported")]
    public string[] ResponseTypesSupported { get; set; } = [];
    
    
    [JsonPropertyName("subject_types_supported")]
    public string[] SubjectTypesSupported { get; set; } = [];
    
    [JsonPropertyName("id_token_signing_alg_values_supported")]
    public string[] IdTokenSigningAlgValuesSupported { get; set; } = [];
    
    [JsonPropertyName("token_endpoint_auth_methods_supported")]
    public string[] TokenEndpointAuthMethodsSupported { get; set; } = [];
    
    [JsonPropertyName("claims_supported")]
    public string[] ClaimsSupported { get; set; } = [];
    
    
    [JsonPropertyName("scopes_supported")]
    public string[] ScopesSupported { get; set; } = [];
    
    [JsonPropertyName("code_challenge_methods_supported")]
    public string[] CodeChallengeMethodsSupported { get; set; } = [];
    
    
    [JsonPropertyName("backchannel_logout_supported")]
    public bool BackchannelLogoutSupported { get; set; }
    
    [JsonPropertyName("backchannel_logout_session_supported")]
    public bool BackchannelLogoutSessionSupported { get; set; }
    
    
    [JsonPropertyName("frontchannel_logout_supported")]
    public bool FrontchannelLogoutSupported { get; set; }
    
    [JsonPropertyName("frontchannel_logout_session_supported")]
    public bool FrontchannelLogoutSessionSupported { get; set; }
}