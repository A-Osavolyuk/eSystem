using System.Text.Json.Serialization;

namespace eShop.Domain.Types;

public class ReplyingParty
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("id")]
    public required string Domain { get; set; }
}