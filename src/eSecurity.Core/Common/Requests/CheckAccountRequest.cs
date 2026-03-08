namespace eSecurity.Core.Common.Requests;

public sealed class CheckAccountRequest
{
    [JsonPropertyName("login")]
    public required string Login { get; set; }
}