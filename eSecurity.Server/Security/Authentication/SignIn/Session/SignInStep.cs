namespace eSecurity.Server.Security.Authentication.SignIn.Session;

public enum SignInStep
{
    Password,
    DeviceTrust,
    TwoFactor,
    OAuth,
    Passkey,
    Complete
}