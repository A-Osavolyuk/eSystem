using eSecurity.Core.Common.Requests;
using eSystem.Core.Security.Authentication.Oidc.Token;

namespace eSecurity.Client.Security.Authentication.Oidc;

public class ConnectService(IApiClient apiClient) : IConnectService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> GetPublicKeysAsync()
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = "/api/v1/Connect/.well-known/jwks.json",
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> GetOpenidConfigurationAsync()
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = "/api/v1/Connect/.well-known/openid-configuration",
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> GetClientInfoAsync(string clientId)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/Connect/clients/{clientId}",
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> AuthorizeAsync(AuthorizeRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/Connect/authorize",
                Data = request
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> TokenAsync(TokenRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/Connect/token",
                Data = request
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.XwwwFormUrlEncoded,
                Authentication = AuthenticationType.Basic
            });

    public async ValueTask<ApiResponse> LogoutAsync(LogoutRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/Connect/logout",
                Data = request
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}