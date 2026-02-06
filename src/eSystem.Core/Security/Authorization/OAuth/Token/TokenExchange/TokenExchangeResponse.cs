using System.Text.Json.Serialization;
using eSystem.Core.Security.Identity.Claims;

namespace eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;

public sealed class TokenExchangeResponse : TokenResponse
{
    [JsonPropertyName("issued_token_type")]
    public required string IssuedTokenType { get; set; }
    
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
    
    [JsonPropertyName("audience")]
    public string? Audience { get; set; }
    
    [JsonPropertyName("issued_at")]
    public long? IssuedAt { get; set; }
}