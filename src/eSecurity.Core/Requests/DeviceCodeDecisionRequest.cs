using System.Text.Json.Serialization;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Core.Requests;

public sealed class DeviceCodeDecisionRequest
{
    [JsonPropertyName("user_code")]
    public required string UserCode { get; set; }
    
    [JsonPropertyName("decision")]
    public DeviceCodeDecision Decision { get; set; }

    [JsonPropertyName("deny_reason")]
    public string? DenyReason { get; set; }
}