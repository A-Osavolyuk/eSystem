namespace eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

public static class AuthenticationMethods
{
    public const string Password = "pwd";
    public const string MultiFactorAuthentication = "mfa";
    public const string OneTimePassword = "otp";
    public const string SoftwareKey = "swk";
    public const string HardwareKey = "hwk";
    public const string Saml = "saml";
    public const string SmsOtp = "sms";
    public const string KnowledgeBasedAuthentication = "kba";
    public const string Fingerprint = "ftp";
    public const string FaceRecognition = "face";
    public const string IrisScan = "eye";
    public const string Federated = "federated";
    public const string Social = "social";
    public const string EmailVerification = "email";
}