using System.Text.Json.Serialization;

namespace eSystem.Core.Requests.Storage;

public class UploadFilesRequest(IReadOnlyList<IBrowserFile> files, string type, string identifier)
{
    [JsonPropertyName("files")]
    public required IReadOnlyList<IBrowserFile> Files { get; set; } = files;
    
    [JsonPropertyName("identifier")]
    public required string Identifier { get; set; } = identifier;
    
    [JsonPropertyName("type")]
    public required string Type { get; set; } = type;
}