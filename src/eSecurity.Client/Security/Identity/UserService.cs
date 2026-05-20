using eSecurity.Client.Common.Http;

namespace eSecurity.Client.Security.Identity;

public class UserService(IApiClient apiClient) : IUserService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> GetUserVerificationMethodsAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Get,
                Url = $"/api/v1/User/verification/methods"
            });
    

    public async ValueTask<ApiResponse> GetUserEmailsAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Get,
                Url = $"/api/v1/User/emails"
            });

    public async ValueTask<ApiResponse> GetUserDevicesAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Get,
                Url = $"/api/v1/User/devices"
            });

    public async ValueTask<ApiResponse> GetUserLinkedAccountsAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Get,
                Url = $"/api/v1/User/linked-accounts"
            });

    public async ValueTask<ApiResponse> GetUserTwoFactorMethodsAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Get,
                Url = $"/api/v1/User/2fa/methods"
            });

    public async ValueTask<ApiResponse> GetUserLoginMethodsAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Get,
                Url = $"/api/v1/User/login-methods"
            });
}