using System.Text.Json.Serialization;

namespace eSecurity.Core.Common.Requests;

public class TokenRequest
{
    [JsonPropertyName("grant_type")]
    public required string GrantType { get; set; }
    
    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }
    
    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
    
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("client_secret")]
    public string? ClientSecret { get; set; }
    
    [JsonPropertyName("code_verifier")]
    public string? CodeVerifier { get; set; }
    
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}