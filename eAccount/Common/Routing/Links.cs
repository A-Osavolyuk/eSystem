namespace eAccount.Common.Routing;

public static class Links
{
    public const string SignIn = "/account/sign-in";
    public const string AccountUnlockPage = "/account/unlock";
    public const string LockedOutPage = "/account/locked-out";
    public const string RecoverPage = "/account/recover";
    public const string DeviceTrustPage = "/account/device/trust";
    public const string SignUp = "/account/sign-up";
    public const string CompleteRegistrationPage = "/account/register/complete";
    public const string ForgotPasswordPage = "/account/password/forgot";
    public const string ResetPasswordPage = "/account/password/reset";
    public const string RemovePasswordPage = "/account/password/remove";
    public const string PasskeySignInPage = "/account/passkeys/sign-in";
    public const string PasskeyAddPage = "/account/passkeys/add";
    public const string TwoFactorLoginPage = "/account/2fa/sign-in";
    public const string RecoveryCodesViewPage = "/account/2fa/recovery-codes/view";
    public const string RecoveryCodesPrintPage = "/account/2fa/recovery-codes/print";
    public const string EnableTwoFactorPage = "/account/2fa/enable";
    public const string OAuthSignedUpPage = "/account/oauth/signed-up";
    public const string EmailsPage = "/account/emails";
    public const string EmailChangePage = "/account/email/change";
    public const string EmailVerifyPage = "/account/email/verify";
    public const string EmailResetPage = "/account/email/reset";
    public const string EmailManagePage = "/account/email/manage";
    public const string SecurityPage = "/account/security";
    public const string Profile = "/account/profile";
    public const string SettingsPage = "/account/settings";
    public const string DevicesPage = "/account/devices";
    public const string PhoneNumbersPage = "/account/phone-numbers";
    public const string Authorize = "/connect/authorize";
    public const string AuthorizeCallback = "/connect/callback";
    public const string Logout = "/connect/logout";
    public const string LogoutCallback = "/connect/logout/callback";
}