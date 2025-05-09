using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface ITwoFactorService
{
    public ValueTask<Response> GetProvidersAsync();
    public ValueTask<Response> GetProvidersAsync(string email);
    public ValueTask<Response> GetTwoFactorStateAsync(string email);
    public ValueTask<Response> ChangeTwoFactorAuthenticationStateAsync(ChangeTwoFactorStateRequest request);
    public ValueTask<Response> LoginWithTwoFactorAuthenticationAsync(TwoFactorLoginRequest request);
    public ValueTask<Response> SendTwoFactorTokenAsync(SendTwoFactorTokenRequest request);
}