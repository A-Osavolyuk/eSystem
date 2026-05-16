using System.Text.Json.Serialization;

namespace eSystem.Core.Server.Security.Authorization.OAuth.Par;

public sealed class PushedAuthorizationResponse
{
    [JsonPropertyName("request_uri")]
    public required string RequestUri { get; set; }
    
    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }
}