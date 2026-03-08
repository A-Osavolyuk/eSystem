namespace eSecurity.Core.Common.Responses;

public sealed class SignUpResponse
{
    [JsonPropertyName("transaction_id")]
    public Guid TransactionId { get; set; }
}