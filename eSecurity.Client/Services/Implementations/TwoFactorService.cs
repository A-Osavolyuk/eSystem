using eSecurity.Client.Common.Http;
using eSecurity.Client.Services.Interfaces;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Services.Implementations;

public class TwoFactorService(IApiClient apiClient) : ITwoFactorService
{
    private readonly IApiClient apiClient = apiClient;

    public async ValueTask<HttpResponse> EnableAsync(EnableTwoFactorRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/enable"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> DisableAsync(DisableTwoFactorRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/disable"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> PreferAsync(PreferTwoFactorMethodRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/prefer"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GenerateQrCodeAsync(GenerateQrCodeRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/qr-code/generate"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> RegenerateQrCodeAsync(RegenerateQrCodeRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/qr-code/regenerate"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/authenticator/reconfigure"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/authenticator/verify"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/recovery-codes/generate"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/recovery-codes/load"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });
}