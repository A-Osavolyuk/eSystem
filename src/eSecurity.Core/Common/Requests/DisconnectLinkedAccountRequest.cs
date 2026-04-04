using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Enums;

namespace eSecurity.Core.Common.Requests;

public sealed class DisconnectLinkedAccountRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<LinkedAccountType>))]
    public LinkedAccountType Type { get; set; }
}