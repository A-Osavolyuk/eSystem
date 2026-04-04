using eSecurity.Core.Security.Identity;
using eSystem.Core.Enums;

namespace eSecurity.Core.Common.DTOs;

public class UserEmailDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("normalized_email")]
    public string NormalizedEmail { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<EmailType>))]
    public EmailType Type { get; set; }
    
    [JsonPropertyName("is_verified")]
    public bool IsVerified { get; set; }

    [JsonPropertyName("varified_at")]
    public DateTimeOffset? VerifiedAt { get; set; }
    
    [JsonPropertyName("update_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}