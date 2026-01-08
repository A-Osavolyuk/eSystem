namespace eSecurity.Server.Security.Authentication.SignIn;

public enum SignInType
{
    Password,
    Passkey,
    OAuth,
    RecoveryCode,
    DeviceTrust,
    TwoFactor
}