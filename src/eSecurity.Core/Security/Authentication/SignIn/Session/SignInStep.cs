using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Authentication.SignIn.Session;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SignInStep
{
    Password,
    TwoFactor,
    OAuth,
    Passkey,
    Complete
}