using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;
using eSecurity.Core.Requests.Email.Change;
using eSecurity.Core.Requests.Email.Reset;
using eSecurity.Core.Requests.Email.Verification;
using ResetEmailRequest = eSecurity.Core.Requests.Email.Reset.ResetEmailRequest;

namespace eSecurity.Client.Security.Identity;

public interface IEmailService
{
    public ValueTask<ApiResponse> AddEmailAsync(AddEmailRequest request);
    public ValueTask<ApiResponse> CheckEmailAsync(CheckEmailRequest request);
    public ValueTask<ApiResponse> RemoveEmailAsync(RemoveEmailRequest request);
    
    public ValueTask<ApiResponse> SendEmailVerificationAsync(SendEmailVerificationOtpRequest otpRequest);
    public ValueTask<ApiResponse> VerifyEmailAsync(VerifyEmailRequest request);

    public ValueTask<ApiResponse> SendEmailChangeAsync(SendEmailChangeOtpRequest otpRequest);
    public ValueTask<ApiResponse> ChangeEmailAsync(ChangeEmailRequest request);
    
    public ValueTask<ApiResponse> SendEmailResetAsync(SendEmailResetOtpRequest otpRequest);
    public ValueTask<ApiResponse> ResetEmailAsync(ResetEmailRequest request);
}