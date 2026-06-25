using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Authentication.TwoFactor;

public class TwoFactorService(IApiClient apiClient) : ITwoFactorService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> EnableAsync(EnableTwoFactorRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/two-factor/enable",
                Data = request
            });

    public async ValueTask<ApiResponse> DisableAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/two-factor/disable"
            });

    public async ValueTask<ApiResponse> PreferAsync(PreferTwoFactorMethodRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/two-factor/prefer"
            });

    public async ValueTask<ApiResponse> GenerateQrCodeAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/two-factor/qr-code/generate"
            });

    public async ValueTask<ApiResponse> RegenerateQrCodeAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/two-factor/qr-code/regenerate"
            });

    public async ValueTask<ApiResponse> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/two-factor/authenticator/reconfigure"
            });

    public async ValueTask<ApiResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/two-factor/authenticator/verify"
            });

    public async ValueTask<ApiResponse> GenerateRecoveryCodesAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/two-factor/recovery-codes/generate"
            });

    public async ValueTask<ApiResponse> LoadRecoveryCodesAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/two-factor/recovery-codes/load"
            });
}