using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.DTOs;

namespace eSecurity.Client.Security.Identity;

public class UserService(IApiClient apiClient) : IUserService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse<UserVerificationData>> GetUserVerificationMethodsAsync(Guid id)
        => await _apiClient.SendAsync<UserVerificationData>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/verification/methods"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });


    public async ValueTask<HttpResponse<UserEmailDto>> GetUserPrimaryEmailAsync(Guid id)
        => await _apiClient.SendAsync<UserEmailDto>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/emails/primary"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse<List<UserEmailDto>>> GetUserEmailsAsync(Guid id)
        => await _apiClient.SendAsync<List<UserEmailDto>>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/emails"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse<UserDeviceDto>> GetUserDeviceAsync(Guid id, Guid deviceId)
        => await _apiClient.SendAsync<UserDeviceDto>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/devices/{deviceId}"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse<List<UserDeviceDto>>> GetUserDevicesAsync(Guid id)
        => await _apiClient.SendAsync<List<UserDeviceDto>>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/devices"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse<UserLinkedAccountData>> GetUserLinkedAccountsAsync(Guid id)
        => await _apiClient.SendAsync<UserLinkedAccountData>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/linked-accounts"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse<List<UserTwoFactorMethod>>> GetUserTwoFactorMethodsAsync(Guid id)
        => await _apiClient.SendAsync<List<UserTwoFactorMethod>>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/2fa/methods"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse<UserLoginMethodsDto>> GetUserLoginMethodsAsync(Guid id)
        => await _apiClient.SendAsync<UserLoginMethodsDto>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/login-methods"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}