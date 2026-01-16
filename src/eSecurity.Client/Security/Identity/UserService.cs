using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.DTOs;

namespace eSecurity.Client.Security.Identity;

public class UserService(IApiClient apiClient) : IUserService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse<UserVerificationData>> GetUserVerificationMethodsAsync(Guid id)
        => await _apiClient.SendAsync<UserVerificationData>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/verification/methods"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });


    public async ValueTask<ApiResponse<UserEmailDto>> GetUserPrimaryEmailAsync(Guid id)
        => await _apiClient.SendAsync<UserEmailDto>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/emails/primary"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse<List<UserEmailDto>>> GetUserEmailsAsync(Guid id)
        => await _apiClient.SendAsync<List<UserEmailDto>>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/emails"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse<UserDeviceDto>> GetUserDeviceAsync(Guid id, Guid deviceId)
        => await _apiClient.SendAsync<UserDeviceDto>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/devices/{deviceId}"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse<List<UserDeviceDto>>> GetUserDevicesAsync(Guid id)
        => await _apiClient.SendAsync<List<UserDeviceDto>>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/devices"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse<UserLinkedAccountData>> GetUserLinkedAccountsAsync(Guid id)
        => await _apiClient.SendAsync<UserLinkedAccountData>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/linked-accounts"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse<List<UserTwoFactorMethod>>> GetUserTwoFactorMethodsAsync(Guid id)
        => await _apiClient.SendAsync<List<UserTwoFactorMethod>>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/2fa/methods"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse<UserLoginMethodsDto>> GetUserLoginMethodsAsync(Guid id)
        => await _apiClient.SendAsync<UserLoginMethodsDto>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}/login-methods"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse<UserDto>> GetUserAsync(Guid id)
        => await _apiClient.SendAsync<UserDto>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/User/{id}"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}