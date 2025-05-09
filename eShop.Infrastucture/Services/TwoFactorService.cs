using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class TwoFactorService(
    IHttpClientService client, 
    IConfiguration configuration) : ApiService(configuration, client), ITwoFactorService
{
    public async ValueTask<Response> GetProvidersAsync() => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/get-providers",
            Method: HttpMethod.Get));

    public async ValueTask<Response> GetProvidersAsync(string email) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/get-user-providers/{email}",
            Method: HttpMethod.Get));
    
    
    public async ValueTask<Response> GetTwoFactorStateAsync(string email) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/get-2fa-state/{email}",
            Method: HttpMethod.Get));
    
    public async ValueTask<Response>
        LoginWithTwoFactorAuthenticationAsync(TwoFactorLoginRequest request) =>
        await HttpClientService.SendAsync(new Request(
            Url: $"{Configuration[Key]}/api/v1/Security/2fa-login",
            Method: HttpMethod.Post,
            Data: request));

    public async ValueTask<Response> SendTwoFactorTokenAsync(SendTwoFactorTokenRequest request) =>
        await HttpClientService.SendAsync(new Request(
            Url: $"{Configuration[Key]}/api/v1/Security/send-token",
            Method: HttpMethod.Post,
            Data: request));

    public async ValueTask<Response> ChangeTwoFactorAuthenticationStateAsync(
        ChangeTwoFactorStateRequest request) => await HttpClientService.SendAsync(
        new Request(Url: $"{Configuration[Key]}/api/v1/Security/change-2fa-state",
            Method: HttpMethod.Post, Data: request));
}