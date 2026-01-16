using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security;

public class SecurityService(IApiClient apiClient) : ISecurityService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse<SignInResponse>> SignInAsync(SignInRequest request)
        => await _apiClient.SendAsync<SignInResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/sign-in"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None,
                WithLocale = true,
                WithTimezone = true
            });

    public async ValueTask<ApiResponse<SignUpResponse>> SignUpAsync(SignUpRequest request)
        => await _apiClient.SendAsync<SignUpResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/sign-up"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None,
                WithLocale = true,
                WithTimezone = true
            });

    public async ValueTask<ApiResponse<SignInSessionDto>> LoadSignInSessionAsync(Guid sid)
        => await _apiClient.SendAsync<SignInSessionDto>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = $"api/v1/Account/sign-in/session/{sid}"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse<CheckAccountResponse>> CheckAccountAsync(CheckAccountRequest request)
        => await _apiClient.SendAsync<CheckAccountResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/check"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse> RecoverAccountAsync(RecoverAccountRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/recover"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse> UnlockAccountAsync(UnlockAccountRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/unlock"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}