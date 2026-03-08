namespace eSecurity.Core.Common.Requests;

public sealed class ChangePasskeyNameRequest
{
    [JsonPropertyName("passkey_id")]
    public required Guid PasskeyId { get; set; }
    
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
}