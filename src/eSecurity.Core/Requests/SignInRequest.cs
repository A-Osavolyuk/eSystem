using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.SignIn;

namespace eSecurity.Core.Requests;

public sealed class SignInRequest
{
    [JsonPropertyName("payload")]
    public required SignInPayload Payload { get; set; }
}