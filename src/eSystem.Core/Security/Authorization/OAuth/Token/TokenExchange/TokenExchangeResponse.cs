using System.Text.Json.Serialization;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;

public sealed class TokenExchangeResponse : TokenResponse
{
    [JsonPropertyName("issued_token_type")]
    [JsonConverter(typeof(JsonPreferredEnumValueConverter<TokenType>))]
    public required TokenType IssuedTokenType { get; set; }
    
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
    
    [JsonPropertyName("audience")]
    public string? Audience { get; set; }
    
    [JsonPropertyName("issued_at")]
    public long? IssuedAt { get; set; }
}