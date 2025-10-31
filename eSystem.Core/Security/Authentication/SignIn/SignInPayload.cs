using System.Text.Json.Serialization;
using eSystem.Core.Security.Authentication.SignIn.Payloads;

namespace eSystem.Core.Security.Authentication.SignIn;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PasswordSignInPayload), "password")]
[JsonDerivedType(typeof(AuthenticatorSignInPayload), "authenticator")]
[JsonDerivedType(typeof(PasskeySignInPayload), "passkey")]
public abstract class SignInPayload { }