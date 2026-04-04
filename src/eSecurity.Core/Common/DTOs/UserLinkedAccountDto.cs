using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Enums;

namespace eSecurity.Core.Common.DTOs;

public class UserLinkedAccountDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<LinkedAccountType>))]
    public LinkedAccountType Type { get; set; }
    
    [JsonPropertyName("linked_at")]
    public DateTimeOffset? LinkedAt { get; set; }
}