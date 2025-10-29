using eAccount.Domain.Abstraction.Services;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Requests.Auth;

namespace eAccount.Infrastructure.Implementations;

public class TwoFactorService(
    IApiClient apiClient,
    GatewayOptions gatewayOptions) : ITwoFactorService
{
    private readonly IApiClient apiClient = apiClient;
    private readonly GatewayOptions gatewayOptions = gatewayOptions;
    private const string BasePath = "api/v1/TwoFactor";

    public async ValueTask<HttpResponse> EnableAsync(EnableTwoFactorRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/enable", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> DisableAsync(DisableTwoFactorRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/disable", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/recovery-code/load", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });
    public async ValueTask<HttpResponse> RevokeRecoveryCodesAsync(RevokeRecoveryCodesRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/recovery-code/revoke", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyRecoveryCodeAsync(VerifyRecoveryCodeRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/recovery-code/verify", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });
    
    public async ValueTask<HttpResponse> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/recovery-code/generate", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateQrCodeAsync(GenerateQrCodeRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/qr-code/generate", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> RegenerateQrCodeAsync(RegenerateQrCodeRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/qr-code/regenerate", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> PreferAsync(PreferTwoFactorMethodRequest request) => await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/prefer", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request) => 
        await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/authenticator/reconfigure", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request) => 
        await apiClient.SendAsync(
            new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/authenticator/verify", Method = HttpMethod.Post, Data = request }, 
            new HttpOptions { Type = DataType.Text });
}