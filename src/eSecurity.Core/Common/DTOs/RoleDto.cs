namespace eSecurity.Core.Common.DTOs;

public class RoleDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("normalized_name")]
    public string NormalizedName { get; set; } = string.Empty;
}