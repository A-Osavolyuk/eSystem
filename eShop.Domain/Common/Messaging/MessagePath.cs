namespace eShop.Domain.Common.Messaging;

public class MessagePath
{
    public const string ChangeEmail = "email-change";
    public const string VerifyEmail = "email-verification";
    public const string VerifiedEmail = "email-verified";
    
    public const string ChangePhoneNumber = "phone-number-change";
    public const string VerifyPhoneNumber = "phone-number-verification";
    public const string VerifiedPhoneNumber = "phone-number-verified";
    
    public const string TwoFactorToken = "two-factor-token";
    public const string ResetPassword = "password-reset";
    public const string OAuthRegistration = "oauth-registration";
}