using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authentication.TwoFactor;

public class TwoFactorService(IApiClient apiClient) : ITwoFactorService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> EnableAsync(EnableTwoFactorRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/TwoFactor/enable",
                Data = request
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> DisableAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/TwoFactor/disable"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> PreferAsync(PreferTwoFactorMethodRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/TwoFactor/prefer"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GenerateQrCodeAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/TwoFactor/qr-code/generate"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> RegenerateQrCodeAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/TwoFactor/qr-code/regenerate"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/TwoFactor/authenticator/reconfigure"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/TwoFactor/authenticator/verify"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GenerateRecoveryCodesAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/TwoFactor/recovery-codes/generate"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> LoadRecoveryCodesAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/TwoFactor/recovery-codes/load"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}