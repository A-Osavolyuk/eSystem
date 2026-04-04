namespace eSecurity.Core.Common.DTOs;

public class PermissionDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}