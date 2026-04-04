using System.Text.Json.Serialization;

namespace eSystem.Core.Responses.Storage;

public class UploadFiledResponse
{
    [JsonPropertyName("files")]
    public List<string> Files { get; set; } = [];
}