using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public sealed class BackchannelAuthenticationResponse
{
    [JsonPropertyName("auth_req_id")]
    public required string AuthReqId { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("interval")]
    public int Interval { get; set; }
}