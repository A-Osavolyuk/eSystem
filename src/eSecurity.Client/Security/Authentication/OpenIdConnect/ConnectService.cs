using eSecurity.Client.Common.Http;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect;

public class ConnectService(IApiClient apiClient) : IConnectService
{
    private readonly IApiClient _apiClient = apiClient;
    public async ValueTask<ApiResponse> GetClientInfoAsync(string clientId)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Get,
                Url = $"api/v1/Connect/clients/{clientId}",
            });

    public async ValueTask<ApiResponse> UserInfoAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Get,
                Url = $"api/v1/Connect/userinfo",
            });
}