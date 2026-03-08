namespace eSecurity.Core.Common.Requests;

public sealed class GenerateCreationOptionsRequest
{
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
}