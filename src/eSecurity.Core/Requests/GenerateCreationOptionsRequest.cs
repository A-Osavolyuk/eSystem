using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class GenerateCreationOptionsRequest
{
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
}