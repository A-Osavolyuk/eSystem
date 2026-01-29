namespace eSecurity.Core.Common.Routing;

public static class Links
{
    public static class Common
    {
        public const string SignUp = "/sign-up";
        public const string UnlockAccount = "/unlock-account";
        public const string RecoverAccount = "/recover-account";
        public const string CompleteRegistration = "/complete-registration";
        public const string ForgotPassword = "/forgot-password";
        public const string AccountLocked = "/account-locked";
        public const string Profile = "/profile";
        public const string Login = "/login";
        public const string PasswordSignIn = "/login/password";
        public const string LoginIdentifier = "/login/identifier";
    }

    public static class Settings
    {
        public const string Emails = "/settings/emails";
        public const string Security = "/settings/security";
        public const string Devices = "/settings/devices";
        public const string PhoneNumbers = "/settings/phone-numbers";
        public const string RecoveryCodesView = "/settings/recovery-codes/view";
        public const string RecoveryCodesPrint = "/settings/recovery-codes/print";
        public const string EnableTwoFactor = "/settings/two-factor/enable";
        public const string PasskeyAdd = "/settings/passkeys/add";
        public const string ResetPassword = "/settings/reset-password";
        public const string RemovePassword = "/settings/remove-password";
    }
    
    public static class Connect
    {
        public const string Authorize = "/connect/authorize";
        public const string Logout = "/connect/logout";
        public const string LogoutCallback = "/connect/logout/callback";
        public const string LoggedOut = "/connect/logged-out";
        public const string Consents = "/connect/consents";
        public const string SelectAccount = "/connect/select-account";
    }

    public static class Session
    {
        public const string TwoFactor = "/session/two-factor";
        public const string DeviceTrust = "/session/trust-device";
    }

    public static class Account
    {
        public const string EmailChangePage = "/account/email/change";
        public const string EmailVerifyPage = "/account/email/verify";
        public const string EmailResetPage = "/account/email/reset";
        public const string EmailManagePage = "/account/email/manage";
        public const string SettingsPage = "/account/settings";
    }

    public static class OAuth
    {
        public const string SignUp = "/oauth/sign-up";
        public const string Error = "/oauth/error";
        public const string Handle = "/oauth/handle";
    }
}