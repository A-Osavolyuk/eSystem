using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Core.Common.DTOs;

public class SignIdentity
{
    [JsonPropertyName("claims")]
    public required List<ClaimValue> Claims { get; set; }
    
    [JsonPropertyName("tokens")]
    public required List<AuthenticationToken> Tokens { get; set; }
}

public class ClaimValue
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    
    [JsonPropertyName("value")]
    public required string Value { get; set; }
}