using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Authorization.OAuth.Token.Ciba;

public sealed class CibaResponse : TokenResponse
{
    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
    
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}