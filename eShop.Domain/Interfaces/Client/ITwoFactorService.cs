using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface ITwoFactorService
{
    public ValueTask<Response> GetProvidersAsync();
    public ValueTask<Response> LoginAsync(TwoFactorLoginRequest request);
    public ValueTask<Response> SendCodeAsync(SendTwoFactorCodeRequest request);
    public ValueTask<Response> VerifyCodeAsync(VerifyTwoFactorCodeRequest request);
    public ValueTask<Response> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    public ValueTask<Response> GenerateQrCodeAsync(GenerateQrCodeRequest request);
}