namespace eSecurity.Server.Security.Authentication.SignIn;

public enum SignInType
{
    Password,
    AuthenticatorApp,
    Passkey,
    OAuth,
    RecoveryCode,
    DeviceTrust,
    TwoFactor
}