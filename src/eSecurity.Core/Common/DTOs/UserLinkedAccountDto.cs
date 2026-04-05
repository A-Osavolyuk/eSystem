using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.DTOs;

public class UserLinkedAccountDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("type")]
    public LinkedAccountType Type { get; set; }
    
    [JsonPropertyName("linked_at")]
    public DateTimeOffset? LinkedAt { get; set; }
}