namespace eSecurity.Core.Common.Requests;

public sealed class SetPasswordRequest
{
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}