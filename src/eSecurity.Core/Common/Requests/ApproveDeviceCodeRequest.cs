namespace eSecurity.Core.Common.Requests;

public sealed class ApproveDeviceCodeRequest
{
    [JsonPropertyName("user_code")]
    public required string UserCode { get; set; }
    
    [JsonPropertyName("session_id")]
    public Guid? SessionId { get; set; }
}