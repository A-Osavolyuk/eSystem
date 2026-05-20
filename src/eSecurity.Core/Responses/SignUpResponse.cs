using System.Text.Json.Serialization;

namespace eSecurity.Core.Responses;

public sealed class SignUpResponse
{
    [JsonPropertyName("transaction_id")]
    public Guid TransactionId { get; set; }
}