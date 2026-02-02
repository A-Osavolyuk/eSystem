namespace eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

public static class AuthenticationMethods
{
    public const string Pwd = "pwd";
    public const string Mfa = "mfa";
    public const string Otp = "otp";
    public const string Swk = "swk";
    public const string Hwk = "hwk";
    public const string Saml = "saml";

    public static class OAuth
    {
        public const string Google = "oauth:google";
        public const string Microsoft = "oauth:mircosoft";
        public const string X = "oauth:x";
        public const string Facebook = "oauth:facebook";
    }
}