namespace eShop.Auth.Api.Interfaces;

public interface ISecurityManager
{
    public string GenerateRandomPassword(int length);
    public ValueTask<string> GenerateVerificationCodeAsync(string sentTo, VerificationCodeType verificationCodeType);
    public ValueTask<CodeSet> GenerateVerificationCodeSetAsync(DestinationSet destinationSet, VerificationCodeType verificationCodeType);
    public ValueTask<IdentityResult> VerifyEmailAsync(AppUser user, string code);
    public ValueTask<IdentityResult> VerifyPhoneNumberAsync(AppUser user, string code);
    public ValueTask<IdentityResult> ResetPasswordAsync(AppUser user, string code, string password);
    public ValueTask<IdentityResult> ChangeEmailAsync(AppUser user, string newEmail, CodeSet codeSet);
    public ValueTask<IdentityResult> ChangePhoneNumberAsync(AppUser user, string newPhoneNumber, CodeSet codeSet);
    public ValueTask<CodeEntity?> FindCodeAsync(string sentTo, VerificationCodeType verificationCodeType);
    public ValueTask<IdentityResult> VerifyCodeAsync(string code, string sentTo, VerificationCodeType codeType);
    public ValueTask<SecurityTokenEntity?> FindTokenAsync(AppUser user);
    public ValueTask<IdentityResult> RemoveTokenAsync(AppUser user);
    public ValueTask SaveTokenAsync(AppUser user, string token, DateTime tokenExpiration);
}