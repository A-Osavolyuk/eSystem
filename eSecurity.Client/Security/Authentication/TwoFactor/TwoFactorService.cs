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
            }, new HttpOptions()
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
            }, new HttpOptions()
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
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse<QrCode>> GenerateQrCodeAsync(GenerateQrCodeRequest request)
        => await _apiClient.SendAsync<QrCode>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/qr-code/generate"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse<QrCode>> RegenerateQrCodeAsync(RegenerateQrCodeRequest request)
        => await _apiClient.SendAsync<QrCode>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/qr-code/regenerate"
            }, new HttpOptions()
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
            }, new HttpOptions()
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
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse<List<string>>> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request)
        => await _apiClient.SendAsync<List<string>>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/recovery-codes/generate"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse<List<string>>> LoadRecoveryCodesAsync(LoadRecoveryCodesRequest request)
        => await _apiClient.SendAsync<List<string>>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/TwoFactor/recovery-codes/load"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}