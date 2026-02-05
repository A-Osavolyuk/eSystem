using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;

public sealed class RefreshTokenResponse : TokenResponse
{
    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}