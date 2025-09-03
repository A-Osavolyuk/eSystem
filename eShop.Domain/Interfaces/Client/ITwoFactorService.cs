using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface ITwoFactorService
{
    public ValueTask<HttpResponse> GetProvidersAsync();
    public ValueTask<HttpResponse> LoginAsync(TwoFactorLoginRequest request);
    public ValueTask<HttpResponse> SendCodeAsync(SendTwoFactorCodeRequest request);
    public ValueTask<HttpResponse> VerifyCodeAsync(VerifyTwoFactorCodeRequest request);
    public ValueTask<HttpResponse> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    public ValueTask<HttpResponse> GenerateQrCodeAsync(GenerateQrCodeRequest request);
}