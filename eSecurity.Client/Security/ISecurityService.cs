using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security;

public interface ISecurityService
{
    public ValueTask<HttpResponse<SignInResponse>> SignInAsync(SignInRequest request);
    public ValueTask<HttpResponse<SignUpResponse>> SignUpAsync(SignUpRequest request);
    
    public ValueTask<HttpResponse<SignInSessionDto>> GetSignInSessionAsync(Guid sid);
    
    public ValueTask<HttpResponse> AddEmailAsync(AddEmailRequest request);
    public ValueTask<HttpResponse> CheckEmailAsync(CheckEmailRequest request);
    public ValueTask<HttpResponse> ChangeEmailAsync(ChangeEmailRequest request);
    public ValueTask<HttpResponse> VerifyEmailAsync(VerifyEmailRequest request);
    public ValueTask<HttpResponse> ManageEmailAsync(ManageEmailRequest request);
    public ValueTask<HttpResponse> RemoveEmailAsync(RemoveEmailRequest request);
    public ValueTask<HttpResponse> ResetEmailAsync(ResetEmailRequest request);
    
    public ValueTask<HttpResponse> ChangeUsernameAsync(ChangeUsernameRequest request);
    public ValueTask<HttpResponse<CheckAccountResponse>> CheckAccountAsync(CheckAccountRequest request);
    public ValueTask<HttpResponse> RecoverAccountAsync(RecoverAccountRequest request);
    public ValueTask<HttpResponse> UnlockAccountAsync(UnlockAccountRequest request);
    
    public ValueTask<HttpResponse> AddPasswordAsync(AddPasswordRequest request);
    public ValueTask<HttpResponse<ForgotPasswordResponse>> ForgotPasswordAsync(ForgotPasswordRequest request);
    public ValueTask<HttpResponse> ResetPasswordAsync(ResetPasswordRequest request);
    public ValueTask<HttpResponse> ChangePasswordAsync(ChangePasswordRequest request);
}