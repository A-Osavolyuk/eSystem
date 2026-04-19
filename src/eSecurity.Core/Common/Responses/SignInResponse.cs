namespace eSecurity.Core.Common.Responses;

public sealed class SignInResponse
{
    [JsonPropertyName("transaction_id")]
    public Guid TransactionId { get; set; }
    
    [JsonPropertyName("session_cookie")]
    public string? SessionCookie { get; set; }
}