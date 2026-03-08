namespace eSecurity.Core.Security.Authentication.TwoFactor;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AuthenticatorTwoFactorPayload), typeDiscriminator: "authenticator")]
[JsonDerivedType(typeof(PasskeyTwoFactorPayload), typeDiscriminator: "passkey")]
[JsonDerivedType(typeof(PasskeyTwoFactorPayload), typeDiscriminator: "recoveryCode")]
public abstract class TwoFactorPayload
{
    
}