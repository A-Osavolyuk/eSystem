using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class ForgotPasswordRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}