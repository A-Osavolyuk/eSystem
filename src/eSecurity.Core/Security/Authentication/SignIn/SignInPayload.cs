using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Authentication.SignIn;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PasswordSignInPayload), typeDiscriminator: "password")]
[JsonDerivedType(typeof(PasskeySignInPayload), typeDiscriminator: "passkey")]
[JsonDerivedType(typeof(OAuthSignInPayload), typeDiscriminator: "oauth")]
[JsonDerivedType(typeof(TrustDeviceSignInPayload), typeDiscriminator: "trustDevice")]
[JsonDerivedType(typeof(TwoFactorSignInPayload), typeDiscriminator: "twoFactor")]
public abstract class SignInPayload { }