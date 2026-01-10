using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Authentication.SignIn.Session;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SignInStep
{
    Password,
    DeviceTrust,
    TwoFactor,
    OAuth,
    Passkey,
    Complete
}