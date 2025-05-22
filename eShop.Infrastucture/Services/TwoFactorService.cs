using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class TwoFactorService(
    IApiClient client,
    IConfiguration configuration) : ApiService(configuration, client), ITwoFactorService
{
    public async ValueTask<Response> GetProvidersAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/get-providers", Method = HttpMethod.Get }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> GetProvidersAsync(string email) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/get-user-providers/{email}", Method = HttpMethod.Get }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> GetStateAsync(string email) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/get-state/{email}", Method = HttpMethod.Get }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> TwoFactorLoginAsync(TwoFactorLoginRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/login", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> SendTwoFactorTokenAsync(SendTwoFactorTokenRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/send-token", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> ChangeStateAsync(ChangeTwoFactorStateRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/change-state", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

}