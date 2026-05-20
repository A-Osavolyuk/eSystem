using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Identity.Claims;

public sealed class ActorClaim
{
    [JsonPropertyName(AppClaimTypes.Sub)]
    public required string Subject { get; set; }
    
    [JsonPropertyName(AppClaimTypes.Iss)]
    public string? Issuer { get; set; }
    
    [JsonPropertyName(AppClaimTypes.AuthTime)]
    public long? AuthenticationTime { get; set; }
    
    [JsonPropertyName(AppClaimTypes.Acr)]
    public string? Acr { get; set; }

    [JsonPropertyName(AppClaimTypes.ClientId)]
    public string? ClientId { get; set; }
    
    [JsonPropertyName(AppClaimTypes.Act)] 
    public ActorClaim? Actor { get; set; }
}