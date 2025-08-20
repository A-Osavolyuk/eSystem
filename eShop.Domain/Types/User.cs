using System.Text.Json.Serialization;

namespace eShop.Domain.Types;

public class User
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("displayName")]
    public required string DisplayName { get; set; }
}