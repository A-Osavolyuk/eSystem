namespace eSystem.Domain.Security.Authentication.TwoFactor;

public enum TwoFactorMethod
{
    AuthenticatorApp,
    Passkey,
    RecoveryCode,
    Sms
}