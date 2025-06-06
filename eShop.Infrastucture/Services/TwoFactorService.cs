using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class TwoFactorService(
    IApiClient client,
    IConfiguration configuration) : ApiService(configuration, client), ITwoFactorService
{
    public async ValueTask<Response> GetProvidersAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Providers/", Method = HttpMethod.Get }, 
            new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> TwoFactorLoginAsync(TwoFactorLoginRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/login", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> SendTwoFactorTokenAsync(SendTwoFactorTokenRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/send-token", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { ValidateToken = false, WithBearer = false, Type = DataType.Text });

    public async ValueTask<Response> GenerateQrCodeAsync(GenerateQrCodeRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/generate-qr-code", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> ChangeStateAsync(ChangeTwoFactorStateRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/change-state", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

}