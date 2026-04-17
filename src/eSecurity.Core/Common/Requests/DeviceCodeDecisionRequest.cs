using eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;

namespace eSecurity.Core.Common.Requests;

public sealed class DeviceCodeDecisionRequest
{
    [JsonPropertyName("user_code")]
    public required string UserCode { get; set; }
    
    [JsonPropertyName("decision")]
    public DeviceCodeDecision Decision { get; set; }
    
    [JsonPropertyName("session_id")]
    public Guid? SessionId { get; set; }

    [JsonPropertyName("deny_reason")]
    public string? DenyReason { get; set; }
}