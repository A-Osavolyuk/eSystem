using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.Requests;

public sealed class DisconnectLinkedAccountRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("type")]
    public LinkedAccountType Type { get; set; }
}