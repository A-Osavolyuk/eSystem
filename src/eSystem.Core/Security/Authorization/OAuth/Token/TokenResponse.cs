using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Authorization.OAuth.Token;

public abstract class TokenResponse
{
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
    
    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; set; }
    
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
}