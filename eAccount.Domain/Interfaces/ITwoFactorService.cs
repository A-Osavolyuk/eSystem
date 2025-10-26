using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eAccount.Domain.Interfaces;

public interface ITwoFactorService
{
    public ValueTask<HttpResponse> EnableAsync(EnableTwoFactorRequest request);
    public ValueTask<HttpResponse> DisableAsync(DisableTwoFactorRequest request);
    public ValueTask<HttpResponse> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    public ValueTask<HttpResponse> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request);
    public ValueTask<HttpResponse> RevokeRecoveryCodesAsync(RevokeRecoveryCodesRequest request);
    public ValueTask<HttpResponse> VerifyRecoveryCodeAsync(VerifyRecoveryCodeRequest request);
    public ValueTask<HttpResponse> GenerateQrCodeAsync(GenerateQrCodeRequest request);
    public ValueTask<HttpResponse> RegenerateQrCodeAsync(RegenerateQrCodeRequest request);
    public ValueTask<HttpResponse> PreferAsync(PreferTwoFactorMethodRequest request);
    public ValueTask<HttpResponse> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request);
    public ValueTask<HttpResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request);
}