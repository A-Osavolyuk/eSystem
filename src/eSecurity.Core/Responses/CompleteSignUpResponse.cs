using System.Text.Json.Serialization;

namespace eSecurity.Core.Responses;

public sealed class CompleteSignUpResponse
{
    [JsonPropertyName("session_cookie")]
    public string? SessionCookie { get; set; }
}