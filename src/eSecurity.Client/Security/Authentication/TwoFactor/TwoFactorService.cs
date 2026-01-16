using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authentication.TwoFactor;

public class TwoFactorService(IApiClient apiClient) : ITwoFactorService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse> EnableAsync(EnableTwoFactorRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/enable"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> DisableAsync(DisableTwoFactorRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/disable"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> PreferAsync(PreferTwoFactorMethodRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/prefer"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse<QrCode>> GenerateQrCodeAsync(GenerateQrCodeRequest request)
        => await _apiClient.SendAsync<QrCode>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/qr-code/generate"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse<QrCode>> RegenerateQrCodeAsync(RegenerateQrCodeRequest request)
        => await _apiClient.SendAsync<QrCode>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/qr-code/regenerate"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> ReconfigureAuthenticatorAsync(ReconfigureAuthenticatorRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/authenticator/reconfigure"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> VerifyAuthenticatorAsync(VerifyAuthenticatorRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/authenticator/verify"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse<List<string>>> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request)
        => await _apiClient.SendAsync<List<string>>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/recovery-codes/generate"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse<List<string>>> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request)
        => await _apiClient.SendAsync<List<string>>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/recovery-codes/load"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}