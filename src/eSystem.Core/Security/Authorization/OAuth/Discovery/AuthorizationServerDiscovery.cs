using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;

namespace eSystem.Core.Security.Authorization.OAuth.Discovery;

public sealed class AuthorizationServerDiscovery
{
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = string.Empty;

    [JsonPropertyName("authorization_endpoint")]
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; } = string.Empty;
    
    [JsonPropertyName("introspection_endpoint")]
    public string IntrospectionEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("revocation_endpoint")]
    public string RevocationEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("device_authorization_endpoint")]
    public string DeviceAuthorizationEndpoint { get; set; } = string.Empty;
    
    
    [JsonPropertyName("grant_types_supported")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<GrantType>))]
    public GrantType[] GrantTypesSupported { get; set; } = [];
    
    [JsonPropertyName("response_types_supported")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<ResponseType>))]
    public ResponseType[] ResponseTypesSupported { get; set; } = [];
    
    [JsonPropertyName("prompt_values_supported")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<PromptType>))]
    public PromptType[] PromptValuesSupported { get; set; } = [];
    
    
    [JsonPropertyName("subject_types_supported")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<SubjectType>))]
    public SubjectType[] SubjectTypesSupported { get; set; } = [];
    
    [JsonPropertyName("token_endpoint_auth_methods_supported")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<TokenAuthMethod>))]
    public TokenAuthMethod[] TokenEndpointAuthMethodsSupported { get; set; } = [];
    
    [JsonPropertyName("claims_supported")]
    public string[] ClaimsSupported { get; set; } = [];
    
    
    [JsonPropertyName("scopes_supported")]
    public string[] ScopesSupported { get; set; } = [];
    
    [JsonPropertyName("code_challenge_methods_supported")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<ChallengeMethod>))]
    public ChallengeMethod[] CodeChallengeMethodsSupported { get; set; } = [];
}