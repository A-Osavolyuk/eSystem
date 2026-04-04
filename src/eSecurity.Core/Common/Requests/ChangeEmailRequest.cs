using eSecurity.Core.Security.Identity;
using eSystem.Core.Enums;

namespace eSecurity.Core.Common.Requests;

public sealed class ChangeEmailRequest
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<EmailType>))]
    public required EmailType Type { get; set; }
}