using System.Text.Json.Serialization;

namespace eSystem.Core.Responses.Storage;

public class LoadFilesResponse
{
    [JsonPropertyName("filed")]
    public List<string> Files { get; set; } = [];
}