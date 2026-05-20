using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.DTOs;

public class UserLinkedAccountDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("type")]
    public LinkedAccountType Type { get; set; }
    
    [JsonPropertyName("linked_at")]
    public DateTimeOffset? LinkedAt { get; set; }
}