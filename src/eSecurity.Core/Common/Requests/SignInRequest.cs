using eSecurity.Core.Security.Authentication.SignIn;

namespace eSecurity.Core.Common.Requests;

public sealed class SignInRequest
{
    [JsonPropertyName("payload")]
    public required SignInPayload Payload { get; set; }
}