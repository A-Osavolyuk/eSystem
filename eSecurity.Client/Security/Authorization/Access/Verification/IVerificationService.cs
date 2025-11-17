using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.Access.Verification;

public interface IVerificationService
{
    public ValueTask<Result> SendCodeAsync(SendCodeRequest request);
    public ValueTask<Result> ResendCodeAsync(ResendCodeRequest request);
    public ValueTask<Result> VerifyCodeAsync(VerifyCodeRequest request);
    public ValueTask<Result> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeRequest request);
    public ValueTask<Result> VerifyPasskeyAsync(VerifyPasskeyRequest request);
}