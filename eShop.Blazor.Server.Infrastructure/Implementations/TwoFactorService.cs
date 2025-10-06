using eShop.Blazor.Server.Domain.Abstraction.Services;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.Auth;

namespace eShop.Blazor.Server.Infrastructure.Implementations;

public class TwoFactorService(
    IApiClient client,
    IConfiguration configuration) : ApiService(configuration, client), ITwoFactorService
{
    public async ValueTask<HttpResponse> LoginAsync(TwoFactorLoginRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/login", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> EnableAsync(EnableTwoFactorRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/enable", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> DisableAsync(DisableTwoFactorRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/disable", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> RevokeRecoveryCodesAsync(RevokeRecoveryCodesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/recovery-code/revoke", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateQrCodeAsync(GenerateQrCodeRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/qr-code/generate", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/recovery-code/generate", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });
}