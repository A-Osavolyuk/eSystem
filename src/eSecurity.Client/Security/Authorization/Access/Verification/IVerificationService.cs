using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.Access.Verification;

public interface IVerificationService
{
    public ValueTask<ApiResponse> SendCodeAsync(SendCodeRequest request);
    public ValueTask<ApiResponse> ResendCodeAsync(ResendCodeRequest request);
    public ValueTask<ApiResponse> VerifyCodeAsync(VerifyCodeRequest request);
    public ValueTask<ApiResponse> VerifyRecoveryCodeAsync(VerifyRecoveryCodeRequest request);
    public ValueTask<ApiResponse> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeRequest request);
    public ValueTask<ApiResponse> VerifyPasskeyAsync(VerifyPasskeyRequest request);
}