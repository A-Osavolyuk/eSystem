using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security;

public interface ISecurityService
{
    public ValueTask<Result> SignInAsync(SignInRequest request);
    public ValueTask<Result> SignUpAsync(SignUpRequest request);
    
    public ValueTask<Result> AddEmailAsync(AddEmailRequest request);
    public ValueTask<Result> CheckEmailAsync(CheckEmailRequest request);
    public ValueTask<Result> ChangeEmailAsync(ChangeEmailRequest request);
    public ValueTask<Result> VerifyEmailAsync(VerifyEmailRequest request);
    public ValueTask<Result> ManageEmailAsync(ManageEmailRequest request);
    public ValueTask<Result> RemoveEmailAsync(RemoveEmailRequest request);
    public ValueTask<Result> ResetEmailAsync(ResetEmailRequest request);
    
    public ValueTask<Result> ChangeUsernameAsync(ChangeUsernameRequest request);
    public ValueTask<Result> CheckAccountAsync(CheckAccountRequest request);
    public ValueTask<Result> RecoverAccountAsync(RecoverAccountRequest request);
    public ValueTask<Result> UnlockAccountAsync(UnlockAccountRequest request);
    
    public ValueTask<Result> AddPasswordAsync(AddPasswordRequest request);
    public ValueTask<Result> ForgotPasswordAsync(ForgotPasswordRequest request);
    public ValueTask<Result> ResetPasswordAsync(ResetPasswordRequest request);
    public ValueTask<Result> ChangePasswordAsync(ChangePasswordRequest request);
}