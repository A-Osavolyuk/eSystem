using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Authorization.Verification;

public interface IVerificationService
{
    public ValueTask<ApiResponse> SendCodeAsync(SendCodeRequest request);
    public ValueTask<ApiResponse> ResendCodeAsync(ResendCodeRequest request);
    public ValueTask<ApiResponse> VerifyAsync(VerificationRequest request);
}