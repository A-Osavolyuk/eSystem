using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Authorization.Verification;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TotpVerificationPayload), typeDiscriminator: "totp")]
[JsonDerivedType(typeof(PasskeyVerificationPayload), typeDiscriminator: "passkey")]
[JsonDerivedType(typeof(AuthenticatorAppVerificationPayload), typeDiscriminator: "authenticator_app")]
public abstract class VerificationPayload { }