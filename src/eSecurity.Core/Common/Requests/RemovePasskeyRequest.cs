namespace eSecurity.Core.Common.Requests;

public sealed class RemovePasskeyRequest
{
    [JsonPropertyName("passkey_id")]
    public Guid PasskeyId { get; set; }
}