namespace eSystem.Core.Security.Cryptography.Protection.Constants;

public static class ProtectionPurposes
{
    public const string Secret = "TOTP.Secret";
    public const string RecoveryCode = "2FA.RecoveryCode";
    public const string Session = "SSO.Session";
    public const string RefreshToken = "SSO.RefreshToken";
    public const string Password = "JWT.PrivateKey.Password";
    public const string Certificate = "JWT.PrivateKey.Certificate";
}