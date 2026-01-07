using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security;

public class SecurityService(IApiClient apiClient) : ISecurityService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse<SignInResponse>> SignInAsync(SignInRequest request)
        => await _apiClient.SendAsync<SignInResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/sign-in"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None,
                WithLocale = true,
                WithTimezone = true
            });

    public async ValueTask<HttpResponse<SignUpResponse>> SignUpAsync(SignUpRequest request)
        => await _apiClient.SendAsync<SignUpResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/sign-up"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None,
                WithLocale = true,
                WithTimezone = true
            });

    public async ValueTask<HttpResponse<SignInSessionDto>> GetSignInSessionAsync(Guid sid)
        => await _apiClient.SendAsync<SignInSessionDto>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = $"api/v1/Account/sign-in/session/{sid}"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse<CheckAccountResponse>> CheckAccountAsync(CheckAccountRequest request)
        => await _apiClient.SendAsync<CheckAccountResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Account/check"
            }, new HttpOptions()
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
            }, new HttpOptions()
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
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}