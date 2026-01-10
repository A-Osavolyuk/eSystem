using System.Text.Json.Serialization;

namespace eSecurity.Core.Common.Requests;

public class LogoutRequest
{
    [JsonPropertyName("id_token_hint")]
    public required string IdTokenHint { get; set; }
    
    [JsonPropertyName("post_logout_redirect_uri")]
    public required string PostLogoutRedirectUri { get; set; }
    
    [JsonPropertyName("state")]
    public required string State { get; set; }
    
    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }
}