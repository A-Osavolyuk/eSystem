using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class ChangeSoftwareKeyNameRequest
{
    [JsonPropertyName("passkey_id")]
    public required Guid PasskeyId { get; set; }
    
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
}