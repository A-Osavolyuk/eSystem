using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;
using eSecurity.Core.Requests.Verification;
using VerificationRequest = eSecurity.Core.Requests.VerificationRequest;

namespace eSecurity.Client.Security.Authorization.Verification;

public interface IVerificationService
{
    public ValueTask<ApiResponse> SendCodeAsync(SendCodeRequest request);
    public ValueTask<ApiResponse> ResendCodeAsync(ResendCodeRequest request);
    public ValueTask<ApiResponse> VerifyAsync(VerificationRequest request);
    public ValueTask<ApiResponse> SendEmailOtpAsync();
    public ValueTask<ApiResponse> ResendEmailOtpAsync();
    public ValueTask<ApiResponse> VerifyEmailOtpAsync(VerifyEmailOtpRequest request);
    public ValueTask<ApiResponse> VerifyAuthenticatorAppAsync(VerifyAuthenticatorAppRequest request);
    public ValueTask<ApiResponse> VerifySoftwareKeyAsync(VerifySoftwareKeyRequest request);
}