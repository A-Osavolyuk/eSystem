using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;

public sealed class OpenIdConfiguration
{
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = string.Empty;
    
    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; } = string.Empty;
    
    [JsonPropertyName("introspection_endpoint")]
    public string IntrospectionEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("revocation_endpoint")]
    public string RevocationEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("device_authorization_endpoint")]
    public string DeviceAuthorizationEndpoint { get; set; } = string.Empty;
    

    [JsonPropertyName("authorization_endpoint")]
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("authorization_response_iss_parameter_supported")]
    public bool AuthorizationResponseIssParameterSupported { get; set; }
    
    
    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("token_endpoint_auth_methods_supported")]
    public string[] TokenEndpointAuthMethodsSupported { get; set; } = [];
    
    [JsonPropertyName("token_endpoint_auth_signing_alg_values_supported")]
    public string[] TokenEndpointAuthSigningAlgValuesSupported { get; set; } = [];
    
    
    [JsonPropertyName("userinfo_endpoint")]
    public string UserinfoEndpoint { get; set; } = string.Empty;

    [JsonPropertyName("userinfo_signing_alg_values_supported")]
    public string[] UserInfoSigningAlgValuesSupported { get; set; } = [];
    
    [JsonPropertyName("userinfo_encryption_alg_values_supported")]
    public string[] UserInfoEncryptionAlgValuesSupported { get; set; } = [];
    
    [JsonPropertyName("userinfo_encryption_enc_values_supported")]
    public string[] UserInfoEncryptionEncValuesSupported { get; set; } = [];
    
    
    [JsonPropertyName("end_session_endpoint")]
    public string EndSessionEndpoint { get; set; } = string.Empty;
    
    [JsonPropertyName("check_session_iframe")]
    public string CheckSessionIframe { get; set; } = string.Empty;
    
    [JsonPropertyName("backchannel_logout_supported")]
    public bool BackchannelLogoutSupported { get; set; }
    
    [JsonPropertyName("backchannel_logout_session_supported")]
    public bool BackchannelLogoutSessionSupported { get; set; }
    
    [JsonPropertyName("frontchannel_logout_supported")]
    public bool FrontchannelLogoutSupported { get; set; }
    
    [JsonPropertyName("frontchannel_logout_session_supported")]
    public bool FrontchannelLogoutSessionSupported { get; set; }
    
    
    [JsonPropertyName("grant_types_supported")]
    public string[] GrantTypesSupported { get; set; } = [];
    
    [JsonPropertyName("response_types_supported")]
    public string[] ResponseTypesSupported { get; set; } = [];
    
    [JsonPropertyName("prompt_values_supported")]
    public string[] PromptValuesSupported { get; set; } = [];
    
    [JsonPropertyName("subject_types_supported")]
    public string[] SubjectTypesSupported { get; set; } = [];
    
    [JsonPropertyName("id_token_signing_alg_values_supported")]
    public string[] IdTokenSigningAlgValuesSupported { get; set; } = [];
    
    [JsonPropertyName("id_token_encryption_alg_values_supported")]
    public string[] IdTokenEncryptionAlgValuesSupported { get; set; } = [];

    [JsonPropertyName("id_token_encryption_enc_values_supported")]
    public string[] IdTokenEncryptionEncValuesSupported { get; set; } = [];

    
    [JsonPropertyName("claims_supported")]
    public string[] ClaimsSupported { get; set; } = [];
    
    [JsonPropertyName("claims_locales_supported")]
    public string[] ClaimsLocalesSupported { get; set; } = [];
    
    [JsonPropertyName("claims_parameter_supported")]
    public bool ClaimsParameterSupported { get; set; }
    
    [JsonPropertyName("acr_values_supported")]
    public string[] AcrValuesSupported { get; set; } = [];
    
    [JsonPropertyName("ui_locales_supported")]
    public string[] UiLocalesSupported { get; set; } = [];
    
    [JsonPropertyName("service_documentation")]
    public string ServiceDocumentation { get; set; } = string.Empty;
    
    [JsonPropertyName("op_policy_uri")]
    public string OpPolicyUri { get; set; } = string.Empty;
    
    [JsonPropertyName("op_tos_uri")]
    public string OpTosUri { get; set; } = string.Empty;
    
    [JsonPropertyName("response_modes_supported")]
    public string[] ResponseModesSupported { get; set; } = [];
    
    [JsonPropertyName("display_values_supported")]
    public string[] DisplayValuesSupported { get; set; } = [];
    

    [JsonPropertyName("request_parameter_supported")]
    public bool RequestParameterSupported { get; set; }
    
    [JsonPropertyName("request_uri_parameter_supported")]
    public bool RequestUriParameterSupported { get; set; }
    
    [JsonPropertyName("require_request_uri_registration")]
    public bool RequireRequestUriRegistration { get; set; }

    [JsonPropertyName("request_object_signing_alg_values_supported")]
    public string[] RequestObjectSigningAlgValuesSupported { get; set; } = [];
    
    [JsonPropertyName("request_object_encryption_alg_values_supported")]
    public string[] RequestObjectEncryptionAlgValuesSupported { get; set; } = [];
    
    [JsonPropertyName("request_object_encryption_enc_values_supported")]
    public string[] RequestObjectEncryptionEncValuesSupported { get; set; } = [];
    
    
    [JsonPropertyName("scopes_supported")]
    public string[] ScopesSupported { get; set; } = [];
    
    [JsonPropertyName("code_challenge_methods_supported")]
    public string[] CodeChallengeMethodsSupported { get; set; } = [];
    
}