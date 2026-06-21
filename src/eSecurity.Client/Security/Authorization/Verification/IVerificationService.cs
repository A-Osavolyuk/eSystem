using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests.Verification;

namespace eSecurity.Client.Security.Authorization.Verification;

public interface IVerificationService
{
    public ValueTask<ApiResponse> SendEmailOtpAsync(SendEmailOtpRequest request);
    public ValueTask<ApiResponse> ResendEmailOtpAsync(ResendEmailOtpRequest request);
    public ValueTask<ApiResponse> VerifyEmailOtpAsync(VerifyEmailOtpRequest request);
    public ValueTask<ApiResponse> VerifyAuthenticatorAppAsync(VerifyAuthenticatorAppRequest request);
    public ValueTask<ApiResponse> VerifySoftwareKeyAsync(VerifySoftwareKeyRequest request);
}