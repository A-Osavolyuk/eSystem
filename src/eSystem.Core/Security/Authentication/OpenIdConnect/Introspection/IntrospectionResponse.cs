using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Introspection;

public class IntrospectionResponse
{
    [JsonPropertyName("active")]
    public required bool Active { get; set; }
    
    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
    
    [JsonPropertyName("client_id")]
    public Guid? ClientId { get; set; }
    
    [JsonPropertyName("sub")]
    public string? Subject { get; set; }
    
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("exp")]
    public long? Expiration { get; set; }
    
    [JsonPropertyName("iat")]
    public long? IssuedAt { get; set; }
    
    [JsonPropertyName("nbf")]
    public long? NotBefore { get; set; }
    
    [JsonPropertyName("aud")]
    public string? Audience { get; set; }
    
    [JsonPropertyName("iss")]
    public string? Issuer { get; set; }
    
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    public static IntrospectionResponse Fail() => new IntrospectionResponse() { Active = false };
}