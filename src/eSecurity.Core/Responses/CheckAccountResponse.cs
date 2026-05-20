using System.Text.Json.Serialization;

namespace eSecurity.Core.Responses;

public class CheckAccountResponse
{
    [JsonPropertyName("exists")]
    public bool Exists { get; set; }
}