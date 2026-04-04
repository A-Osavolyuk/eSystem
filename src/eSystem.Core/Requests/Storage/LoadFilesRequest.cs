using System.Text.Json.Serialization;

namespace eSystem.Core.Requests.Storage;

public class LoadFilesRequest
{
    [JsonPropertyName("metadata")]
    public required Metadata Metadata { get; set; }
}