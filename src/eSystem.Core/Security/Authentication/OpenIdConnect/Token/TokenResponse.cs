using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Token;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
    
    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
    
    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }
}