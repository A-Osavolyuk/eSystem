namespace eSecurity.Core.Common.Responses;

public sealed class CompleteSignUpResponse
{
    [JsonPropertyName("session_cookie")]
    public string? SessionCookie { get; set; }
}