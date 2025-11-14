using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security;

public interface ISecurityService
{
    public ValueTask<HttpResponse> SignInAsync(SignInRequest request);
    public ValueTask<HttpResponse> SignUpAsync(SignUpRequest request);
    
    public ValueTask<HttpResponse> AddEmailAsync(AddEmailRequest request);
    public ValueTask<HttpResponse> CheckEmailAsync(CheckEmailRequest request);
    public ValueTask<HttpResponse> ChangeEmailAsync(ChangeEmailRequest request);
    public ValueTask<HttpResponse> VerifyEmailAsync(VerifyEmailRequest request);
    public ValueTask<HttpResponse> ManageEmailAsync(ManageEmailRequest request);
    public ValueTask<HttpResponse> RemoveEmailAsync(RemoveEmailRequest request);
    public ValueTask<HttpResponse> ResetEmailAsync(ResetEmailRequest request);
    
    public ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request);
    public ValueTask<HttpResponse> CheckAccountAsync(CheckAccountRequest request);
    public ValueTask<HttpResponse> RecoverAccountAsync(RecoverAccountRequest request);
    public ValueTask<HttpResponse> UnlockAccountAsync(UnlockAccountRequest request);
    
    public ValueTask<HttpResponse> AddPasswordAsync(AddPasswordRequest request);
    public ValueTask<HttpResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
    public ValueTask<HttpResponse> ResetPasswordAsync(ResetPasswordRequest request);
    public ValueTask<HttpResponse> ChangePasswordAsync(ChangePasswordRequest request);
}