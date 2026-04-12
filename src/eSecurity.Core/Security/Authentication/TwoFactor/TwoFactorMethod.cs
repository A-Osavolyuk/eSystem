using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Core.Security.Authentication.TwoFactor;

[JsonConverter(typeof(JsonEnumValueConverter<TwoFactorMethod>))]
public enum TwoFactorMethod
{
    [EnumValue("authenticator_app")]
    AuthenticatorApp,
    
    [EnumValue("passkey")]
    Passkey,
    
    [EnumValue("recovery_code")]
    RecoveryCode,
    
    [EnumValue("sms")]
    Sms
}