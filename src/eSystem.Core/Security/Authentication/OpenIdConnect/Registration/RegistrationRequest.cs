using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Registration;

public sealed class RegistrationRequest
{
    [JsonPropertyName("client_name")]
    public required string ClientName { get; set; }
    
    [JsonPropertyName("require_pkce")]
    public bool? RequirePkce { get; set; }
    
    [JsonPropertyName("redirect_uris")] 
    public required string[] RedirectUris { get; set; }
    
    [JsonPropertyName("grant_types")] 
    public required string[] GrantTypes { get; set; }
    
    [JsonPropertyName("response_types")] 
    public required string[] ResponseTypes { get; set; }
    
    [JsonPropertyName("scope")] 
    public required string Scope { get; set; }
    
    [JsonPropertyName("token_endpoint_auth_methods")] 
    public required string[] TokenEndpointAuthMethods { get; set; }
    
    [JsonPropertyName("post_logout_redirect_uris")]
    public string[]? PostLogoutRedirectUris { get; set; }

    [JsonPropertyName("frontchannel_logout_uri")]
    public string? FrontchannelLogoutUri { get; set; }

    [JsonPropertyName("frontchannel_logout_session_supported")]
    public bool? FrontchannelLogoutSessionSupported { get; set; }

    [JsonPropertyName("backchannel_logout_uri")]
    public string? BackchannelLogoutUri { get; set; }

    [JsonPropertyName("backchannel_logout_session_supported")]
    public bool? BackchannelLogoutSessionSupported { get; set; }
    
    [JsonPropertyName("subject_type")]
    public string? SubjectType { get; set; }
    
    [JsonPropertyName("sector_identifier_uri")]
    public string? SectorIdentifierUri { get; set; }
    
    [JsonPropertyName("require_auth_time")]
    public bool? RequireAuthTime { get; set; }

    [JsonPropertyName("default_max_age")]
    public int? DefaultMaxAge { get; set; }
    
    [JsonPropertyName("default_acr_values")]
    public string[]? DefaultAcrValues { get; set; }
    
    [JsonPropertyName("request_uris")]
    public string[]? RequestUris { get; set; }
    
    [JsonPropertyName("jwks_uri")] 
    public string? JwksUri { get; set; }
    
    [JsonPropertyName("logo_uri")] 
    public string? LogoUri { get; set; }
    
    [JsonPropertyName("client_uri")] 
    public string? ClientUri { get; set; }
    
    [JsonPropertyName("policy_uri")] 
    public string? PolicyUri { get; set; }
    
    [JsonPropertyName("tos_uri")] 
    public string? TosUri { get; set; }
    
    [JsonPropertyName("contacts")]
    public string[]? Contacts { get; set; }
} 