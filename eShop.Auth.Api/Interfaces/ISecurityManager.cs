namespace eShop.Auth.Api.Interfaces;

public interface ISecurityManager
{
    public string GenerateRandomPassword(int length);
    public ValueTask<string> GenerateVerificationCodeAsync(string destination, VerificationCodeType codeType);
    public ValueTask<CodeSet> GenerateVerificationCodeSetAsync(DestinationSet destinationSet, VerificationCodeType codeType);
    public ValueTask<IdentityResult> VerifyEmailAsync(AppUser user, string code);
    public ValueTask<IdentityResult> VerifyPhoneNumberAsync(AppUser user, string code);
    public ValueTask<IdentityResult> ResetPasswordAsync(AppUser user, string code, string password);
    public ValueTask<IdentityResult> ChangeEmailAsync(AppUser user, string newEmail, CodeSet codeSet);
    public ValueTask<IdentityResult> ChangePhoneNumberAsync(AppUser user, string newPhoneNumber, CodeSet codeSet);
    public ValueTask<CodeEntity?> FindCodeAsync(string destination, VerificationCodeType codeType);
    public ValueTask<IdentityResult> VerifyCodeAsync(string code, string destination, VerificationCodeType codeType);
    public ValueTask<SecurityTokenEntity?> FindTokenAsync(AppUser user);
    public ValueTask<IdentityResult> RemoveTokenAsync(AppUser user);
    public ValueTask SaveTokenAsync(AppUser user, string token, DateTime tokenExpiration);
}