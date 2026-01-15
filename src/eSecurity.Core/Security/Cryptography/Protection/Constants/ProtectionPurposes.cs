namespace eSecurity.Core.Security.Cryptography.Protection.Constants;

public static class ProtectionPurposes
{
    public const string Secret = "TOTP.Secret";
    public const string RecoveryCode = "2FA.RecoveryCode";
    public const string Session = "Session";
    public const string Token = "Token";
    public const string RefreshToken = "eSystem.Authentication.RefreshToken";
    public const string Password = "JWT.PrivateKey.Password";
    public const string Certificate = "JWT.PrivateKey.Certificate";
}