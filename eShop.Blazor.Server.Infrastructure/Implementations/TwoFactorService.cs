using eShop.Blazor.Server.Domain.Abstraction.Services;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Blazor.Server.Infrastructure.Implementations;

public class TwoFactorService(
    IApiClient client,
    IConfiguration configuration) : ApiService(configuration, client), ITwoFactorService
{
    public async ValueTask<HttpResponse> GetProvidersAsync() =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Providers/", Method = HttpMethod.Get }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> LoginAsync(TwoFactorLoginRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/login", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateQrCodeAsync(GenerateQrCodeRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/qr-code/generate", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/recovery-code/generate", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });
}