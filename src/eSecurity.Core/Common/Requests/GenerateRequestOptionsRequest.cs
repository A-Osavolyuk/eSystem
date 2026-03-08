namespace eSecurity.Core.Common.Requests;

public sealed class GenerateRequestOptionsRequest
{
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
}