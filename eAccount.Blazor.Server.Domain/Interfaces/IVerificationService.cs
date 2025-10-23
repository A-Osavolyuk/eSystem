using eSystem.Domain.Common.Http;
using eSystem.Domain.Requests.Auth;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IVerificationService
{
    public ValueTask<HttpResponse> SendCodeAsync(SendCodeRequest request);
    public ValueTask<HttpResponse> ResendCodeAsync(ResendCodeRequest request);
    public ValueTask<HttpResponse> VerifyCodeAsync(VerifyCodeRequest request);
    public ValueTask<HttpResponse> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeRequest request);
    public ValueTask<HttpResponse> VerifyPasskeyAsync(VerifyPasskeyRequest request);
}