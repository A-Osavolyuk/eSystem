namespace eShop.Auth.Api.Interfaces;

public interface ISecurityManager
{
    public string GenerateRandomPassword(int length);
    public ValueTask<string> GenerateVerificationCodeAsync(string destination, Verification codeType);
    public ValueTask<CodeSet> GenerateVerificationCodeSetAsync(DestinationSet destinationSet, Verification codeType);
    public ValueTask<IdentityResult> VerifyEmailAsync(UserEntity userEntity, string code);
    public ValueTask<IdentityResult> VerifyPhoneNumberAsync(UserEntity userEntity, string code);
    public ValueTask<IdentityResult> ResetPasswordAsync(UserEntity userEntity, string code, string password);
    public ValueTask<IdentityResult> ChangeEmailAsync(UserEntity userEntity, string newEmail, CodeSet codeSet);
    public ValueTask<IdentityResult> ChangePhoneNumberAsync(UserEntity userEntity, string newPhoneNumber, CodeSet codeSet);
    public ValueTask<VerificationCodeEntity?> FindCodeAsync(string destination, Verification codeType);
    public ValueTask<IdentityResult> VerifyCodeAsync(string code, string destination, Verification codeType);
    public ValueTask<SecurityTokenEntity?> FindTokenAsync(UserEntity userEntity);
    public ValueTask<IdentityResult> RemoveTokenAsync(UserEntity userEntity);
    public ValueTask SaveTokenAsync(UserEntity userEntity, string token, DateTime tokenExpiration);
}