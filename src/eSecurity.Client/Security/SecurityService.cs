using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security;

public class SecurityService(IApiClient apiClient) : ISecurityService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> SignInAsync(SignInRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Account/sign-in"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None,
                WithLocale = true,
                WithTimezone = true
            });

    public async ValueTask<ApiResponse> SignUpAsync(SignUpRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Account/sign-up"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None,
                WithLocale = true,
                WithTimezone = true
            });

    public async ValueTask<ApiResponse> LoadSignInSessionAsync(Guid sid)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Url = $"api/v1/Account/sign-in/session/{sid}"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> CheckAccountAsync(CheckAccountRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Account/check"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> RecoverAccountAsync(RecoverAccountRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Account/recover"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> UnlockAccountAsync(UnlockAccountRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Account/unlock"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}