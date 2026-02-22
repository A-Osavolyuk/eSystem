namespace eSecurity.Client.Security.Identity;

public class UserService(IApiClient apiClient) : IUserService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> GetUserVerificationMethodsAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/verification/methods"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
    

    public async ValueTask<ApiResponse> GetUserEmailsAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/emails"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserDevicesAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/devices"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserLinkedAccountsAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/linked-accounts"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserTwoFactorMethodsAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/2fa/methods"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserLoginMethodsAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/login-methods"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}