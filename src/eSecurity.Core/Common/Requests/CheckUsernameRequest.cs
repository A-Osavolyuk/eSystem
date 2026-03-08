namespace eSecurity.Core.Common.Requests;

public sealed class CheckUsernameRequest
{
    [JsonPropertyName("username")]
    public required string Username { get; set; }
}