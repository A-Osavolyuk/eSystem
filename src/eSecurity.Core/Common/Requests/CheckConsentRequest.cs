namespace eSecurity.Core.Common.Requests;

public sealed class CheckConsentRequest
{
    [JsonPropertyName("client_id")]
    public required Guid ClientId { get; set; }
    
    [JsonPropertyName("scopes")]
    public required List<string> Scopes { get; set; }
}