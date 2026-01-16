using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Oidc.Client;
using eSystem.Core.Security.Authentication.Oidc;
using eSystem.Core.Security.Authentication.Oidc.Token;

namespace eSecurity.Client.Security.Authentication.Oidc;

public class ConnectService(IApiClient apiClient) : IConnectService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse<JsonWebKeySet>> GetPublicKeysAsync()
        => await _apiClient.SendAsync<JsonWebKeySet>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = "api/v1/Connect/.well-known/jwks.json",
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse<OpenIdConfiguration>> GetOpenidConfigurationAsync()
        => await _apiClient.SendAsync<OpenIdConfiguration>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = "api/v1/Connect/.well-known/openid-configuration",
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse<ClientInfo>> GetClientInfoAsync(string clientId)
        => await _apiClient.SendAsync<ClientInfo>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/Connect/clients/{clientId}",
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse<AuthorizeResponse>> AuthorizeAsync(AuthorizeRequest request)
        => await _apiClient.SendAsync<AuthorizeResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Connect/authorize",
                Data = request
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse<TokenResponse>> TokenAsync(TokenRequest request)
        => await _apiClient.SendAsync<TokenResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Connect/token",
                Data = request
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.XwwwFormUrlEncoded,
                Authentication = AuthenticationType.Basic
            });

    public async ValueTask<ApiResponse<LogoutResponse>> LogoutAsync(LogoutRequest request)
        => await _apiClient.SendAsync<LogoutResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Connect/logout",
                Data = request
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}