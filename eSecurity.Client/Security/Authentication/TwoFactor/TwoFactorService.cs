using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Security.Authentication.TwoFactor;

public class TwoFactorService(IApiClient apiClient) : ITwoFactorService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<Result> EnableAsync(EnableTwoFactorRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/enable"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> DisableAsync(DisableTwoFactorRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/disable"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> PreferAsync(PreferTwoFactorMethodRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/prefer"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> GenerateQrCodeAsync(GenerateQrCodeRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/qr-code/generate"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> RegenerateQrCodeAsync(RegenerateQrCodeRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/qr-code/regenerate"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/authenticator/reconfigure"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/authenticator/verify"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/recovery-codes/generate"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/recovery-codes/load"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });
}