using System.Text.Json.Serialization;
using eSystem.Core.Common.Serialization;

namespace eSystem.Core.Security.Identity.Claims;

public sealed class ActorClaim : IDeep
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

    public int GetDepth()
    {
        var depth = 0;
        var actor = Actor;
        
        while (actor is not null)
        {
            depth++;
            actor = actor.Actor;
        }
        
        return depth;
    }
}