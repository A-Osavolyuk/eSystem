using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;
using eSecurity.Core.Requests.Email.Change;
using eSecurity.Core.Requests.Email.Verification;

namespace eSecurity.Client.Security.Identity;

public interface IEmailService
{
    public ValueTask<ApiResponse> AddEmailAsync(AddEmailRequest request);
    public ValueTask<ApiResponse> CheckEmailAsync(CheckEmailRequest request);
    public ValueTask<ApiResponse> RemoveEmailAsync(RemoveEmailRequest request);
    public ValueTask<ApiResponse> ResetEmailAsync(ResetEmailRequest request);
    
    public ValueTask<ApiResponse> ConfirmEmailVerificationAsync(ConfirmEmailVerificationRequest request);
    public ValueTask<ApiResponse> SendEmailVerificationAsync(SendEmailVerificationRequest request);
    public ValueTask<ApiResponse> ResendEmailVerificationAsync(ResendEmailVerificationRequest request);

    public ValueTask<ApiResponse> SendEmailChangeAsync(SendEmailChangeRequest request);
    public ValueTask<ApiResponse> ResendEmailChangeAsync(ResendEmailChangeRequest request);
    public ValueTask<ApiResponse> ConfirmEmailChangeAsync(ConfirmEmailChangeRequest request);
    public ValueTask<ApiResponse> CanChangeEmailAsync(CanChangeEmailRequest request);
}