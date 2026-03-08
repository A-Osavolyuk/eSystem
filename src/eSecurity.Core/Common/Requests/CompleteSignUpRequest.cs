namespace eSecurity.Core.Common.Requests;

public sealed class CompleteSignUpRequest
{
    [JsonPropertyName("transaction_id")]
    public required Guid TransactionId { get; set; }
    
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}