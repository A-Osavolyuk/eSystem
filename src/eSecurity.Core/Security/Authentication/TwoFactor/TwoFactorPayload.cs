using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Authentication.TwoFactor;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AuthenticatorTwoFactorPayload), typeDiscriminator: "authenticator")]
[JsonDerivedType(typeof(SoftwareKeyTwoFactorPayload), typeDiscriminator: "software_key")]
[JsonDerivedType(typeof(RecoveryCodeTwoFactorPayload), typeDiscriminator: "recovery_code")]
public abstract class TwoFactorPayload {}