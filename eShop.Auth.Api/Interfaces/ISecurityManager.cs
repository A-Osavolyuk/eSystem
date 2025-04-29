namespace eShop.Auth.Api.Interfaces;

public interface ISecurityManager
{
    public string GenerateRandomPassword(int length);
    public ValueTask<Result> ConfirmEmailAsync(UserEntity userEntity, string code);
    public ValueTask<Result> ConfirmPhoneNumberAsync(UserEntity userEntity, string code);
    public ValueTask<Result> ResetPasswordAsync(UserEntity userEntity, string code, string password);
    public ValueTask<Result> ChangeEmailAsync(UserEntity userEntity, string newEmail, CodeSet codeSet);
    public ValueTask<Result> ChangePhoneNumberAsync(UserEntity userEntity, string newPhoneNumber, CodeSet codeSet);
}