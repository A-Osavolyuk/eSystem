namespace eSecurity.Core.Common.Requests;

public sealed class GrantConsentRequest
{
    [JsonPropertyName("session_id")]
    public required Guid SessionId { get; set; }
    
    [JsonPropertyName("client_id")]
    public required Guid ClientId { get; set; }
    
    [JsonPropertyName("scopes")]
    public required List<string> Scopes { get; set; }
}