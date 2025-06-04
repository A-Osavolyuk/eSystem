using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface ITwoFactorService
{
    public ValueTask<Response> GetProvidersAsync();
    public ValueTask<Response> ChangeStateAsync(ChangeTwoFactorStateRequest request);
    public ValueTask<Response> TwoFactorLoginAsync(TwoFactorLoginRequest request);
    public ValueTask<Response> SendTwoFactorTokenAsync(SendTwoFactorTokenRequest request);
    public ValueTask<Response> GenerateQrCodeAsync(GenerateQrCodeRequest request);
}