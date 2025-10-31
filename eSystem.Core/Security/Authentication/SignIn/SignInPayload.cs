using System.Text.Json.Serialization;
using eSystem.Core.Security.Authentication.SignIn.Payloads;

namespace eSystem.Core.Security.Authentication.SignIn;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(PasswordSignInPayload), 0)]
[JsonDerivedType(typeof(AuthenticatorSignInPayload), 1)]
[JsonDerivedType(typeof(PasskeySignInPayload), 2)]
public abstract class SignInPayload
{
    public required SignInType Type { get; set; }
}