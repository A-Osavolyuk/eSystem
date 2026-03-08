namespace eSecurity.Core.Common.Requests;

public sealed class ReconfigureAuthenticatorRequest
{
    [JsonPropertyName("secret")]
    public required string Secret { get; set; }
}