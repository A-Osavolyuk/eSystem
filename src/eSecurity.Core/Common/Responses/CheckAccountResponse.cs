namespace eSecurity.Core.Common.Responses;

public class CheckAccountResponse
{
    [JsonPropertyName("exists")]
    public bool Exists { get; set; }
}