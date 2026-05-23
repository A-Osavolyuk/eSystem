using System.Text.Json.Serialization;

namespace eSecurity.Core.Responses;

public sealed class SignInResponse
{
    [JsonPropertyName("transaction_id")]
    public Guid TransactionId { get; set; }

    [JsonPropertyName("require_2fa")]
    public bool Require2Fa { get; set; }
}