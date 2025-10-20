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
    private const string BasePath = "api/v1/TwoFactor";

    public async ValueTask<HttpResponse> EnableAsync(EnableTwoFactorRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}/enable", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> DisableAsync(DisableTwoFactorRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}/disable", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}/recovery-code/load", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });
    public async ValueTask<HttpResponse> RevokeRecoveryCodesAsync(RevokeRecoveryCodesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}/recovery-code/revoke", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyRecoveryCodeAsync(VerifyRecoveryCodeRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}/recovery-code/verify", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateQrCodeAsync(GenerateQrCodeRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}/qr-code/generate", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> PreferAsync(PreferTwoFactorMethodRequest request) => await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}/prefer", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/{BasePath}/recovery-code/generate", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });
}