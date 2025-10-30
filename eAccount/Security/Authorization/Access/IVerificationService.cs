using eSystem.Core.Requests.Auth;

namespace eAccount.Security.Authorization.Access;

public interface IVerificationService
{
    public ValueTask<HttpResponse> SendCodeAsync(SendCodeRequest request);
    public ValueTask<HttpResponse> ResendCodeAsync(ResendCodeRequest request);
    public ValueTask<HttpResponse> VerifyCodeAsync(VerifyCodeRequest request);
    public ValueTask<HttpResponse> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeRequest request);
    public ValueTask<HttpResponse> VerifyPasskeyAsync(VerifyPasskeyRequest request);
}