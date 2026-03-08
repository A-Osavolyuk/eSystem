namespace eSecurity.Core.Common.Requests;

public sealed class AddPasswordRequest
{
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}