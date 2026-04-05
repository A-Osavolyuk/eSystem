using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Core.Common.Requests;

public sealed class AuthorizeRequest
{
    [JsonPropertyName("session_id")]
    public required Guid SessionId { get; set; }
    
    [JsonPropertyName("response_type")]
    public required ResponseType ResponseType { get; set; }
    
    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }
    
    [JsonPropertyName("redirect_uri")]
    public required string RedirectUri { get; set; }
    
    [JsonPropertyName("scopes")]
    public required List<string> Scopes { get; set; }
    
    [JsonPropertyName("nonce")]
    public required string Nonce { get; set; }
    
    [JsonPropertyName("state")]
    public required string State { get; set; }
    
    [JsonPropertyName("code_challenge")]
    public string? CodeChallenge { get; set; }
    
    [JsonPropertyName("code_challenge_method")]
    public ChallengeMethod? CodeChallengeMethod { get; set; }
}