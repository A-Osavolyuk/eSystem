namespace eSecurity.Client.Security.Identity;

public class UserService(IApiClient apiClient) : IUserService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> GetUserVerificationMethodsAsync(Guid id)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{id}/verification/methods"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });


    public async ValueTask<ApiResponse> GetUserPrimaryEmailAsync(Guid id)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{id}/emails/primary"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> GetUserEmailsAsync(Guid id)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{id}/emails"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserDeviceAsync(Guid id, Guid deviceId)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{id}/devices/{deviceId}"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> GetUserDevicesAsync(Guid id)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{id}/devices"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserLinkedAccountsAsync(Guid id)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{id}/linked-accounts"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserTwoFactorMethodsAsync(Guid id)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{id}/2fa/methods"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> GetUserLoginMethodsAsync(Guid id)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{id}/login-methods"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserAsync(Guid id)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{id}"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}