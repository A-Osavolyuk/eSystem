namespace eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

public class AuthenticationContextClasses
{
    public static class Saml2
    {
        public const string PasswordProtectedTransport = "urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport";
        public const string TimeSyncToken = "urn:oasis:names:tc:SAML:2.0:ac:classes:TimeSyncToken";
        public const string SmartCard = "urn:oasis:names:tc:SAML:2.0:ac:classes:Smartcard";
        public const string MobileTwoFactorContract = "urn:oasis:names:tc:SAML:2.0:ac:classes:MobileTwoFactorContract";
    }

    public static class InCommon
    {
        public const string Bronze = "urn:mace:incommon:iap:bronze";
        public const string Silver = "urn:mace:incommon:iap:silver";
        public const string Gold = "urn:mace:incommon:iap:gold";
    }
}