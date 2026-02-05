using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Authorization.OAuth;

public class OAuthState
{
    [JsonPropertyName("response_type")]
    public string? ResponseType { get; set; }
    
    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }
    
    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; set; }
    
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
    
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
    
    [JsonPropertyName("state")]
    public string? State { get; set; }
    
    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }
    
    [JsonPropertyName("return_url")] 
    public string? ReturnUrl { get; set; }
}