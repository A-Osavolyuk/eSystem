using eShop.Domain.Common.Http;
using eShop.Domain.Requests.Auth;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface ITwoFactorService
{
    public ValueTask<HttpResponse> LoginAsync(TwoFactorLoginRequest request);
    public ValueTask<HttpResponse> EnableAsync(EnableTwoFactorRequest request);
    public ValueTask<HttpResponse> DisableAsync(DisableTwoFactorRequest request);
    public ValueTask<HttpResponse> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    public ValueTask<HttpResponse> RevokeRecoveryCodesAsync(RevokeRecoveryCodesRequest request);
    public ValueTask<HttpResponse> GenerateQrCodeAsync(GenerateQrCodeRequest request);
}