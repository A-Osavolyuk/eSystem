namespace eSecurity.Client.Security.Identity;

public class UserService(IApiClient apiClient) : IUserService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> GetUserVerificationMethodsAsync(string subject)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{subject}/verification/methods"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
    

    public async ValueTask<ApiResponse> GetUserEmailsAsync(string subject)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{subject}/emails"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserDevicesAsync(string subject)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{subject}/devices"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserLinkedAccountsAsync(string subject)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{subject}/linked-accounts"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserTwoFactorMethodsAsync(string subject)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{subject}/2fa/methods"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> GetUserLoginMethodsAsync(string subject)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Get,
                Url = $"/api/v1/User/{subject}/login-methods"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}