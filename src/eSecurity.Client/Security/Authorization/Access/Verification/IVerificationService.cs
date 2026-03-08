using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.Access.Verification;

public interface IVerificationService
{
    public ValueTask<ApiResponse> SendCodeAsync(SendCodeRequest request);
    public ValueTask<ApiResponse> ResendCodeAsync(ResendCodeRequest request);
    public ValueTask<ApiResponse> VerifyAsync(VerificationRequest request);
}