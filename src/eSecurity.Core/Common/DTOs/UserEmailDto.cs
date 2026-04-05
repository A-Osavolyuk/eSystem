using eSecurity.Core.Security.Identity;

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
    public EmailType Type { get; set; }
    
    [JsonPropertyName("is_verified")]
    public bool IsVerified { get; set; }

    [JsonPropertyName("varified_at")]
    public DateTimeOffset? VerifiedAt { get; set; }
    
    [JsonPropertyName("update_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}