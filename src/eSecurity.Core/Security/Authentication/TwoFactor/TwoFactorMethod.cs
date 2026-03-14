using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Authentication.TwoFactor;

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