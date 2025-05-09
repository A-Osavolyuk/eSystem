using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface ITwoFactorService
{
    public ValueTask<Response> ChangeTwoFactorAuthenticationStateAsync(ChangeTwoFactorStateRequest request);
    public ValueTask<Response> GetTwoFactorStateAsync(string email);
    public ValueTask<Response> LoginWithTwoFactorAuthenticationAsync(TwoFactorLoginRequest request);
}