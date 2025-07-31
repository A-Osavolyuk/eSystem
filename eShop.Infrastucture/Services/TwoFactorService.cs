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
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> LoginAsync(TwoFactorLoginRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/login", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> SendCodeAsync(SendTwoFactorCodeRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/code/send", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = false, Type = DataType.Text });
    
    public async ValueTask<Response> VerifyCodeAsync(VerifyTwoFactorCodeRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/code/verify", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/TwoFactor/recovery-code/generate", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { WithBearer = true, Type = DataType.Text });
}