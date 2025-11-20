using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Oidc;
using eSecurity.Core.Security.Authentication.Oidc.Client;

namespace eSecurity.Client.Security.Authentication.Oidc;

public class ConnectService(IApiClient apiClient) : IConnectService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse<JsonWebKeySet>> GetPublicKeysAsync()
        => await _apiClient.SendAsync<JsonWebKeySet>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = "api/v1/Connect/.well-known/jwks.json",
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse<OpenIdOptions>> GetOpenidConfigurationAsync()
        => await _apiClient.SendAsync<OpenIdOptions>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = "api/v1/Connect/.well-known/openid-configuration",
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse<ClientInfo>> GetClientInfoAsync(string clientId)
        => await _apiClient.SendAsync<ClientInfo>(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = $"api/v1/Connect/clients/{clientId}",
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse<AuthorizeResponse>> AuthorizeAsync(AuthorizeRequest request)
        => await _apiClient.SendAsync<AuthorizeResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Connect/authorize",
                Data = request
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse<TokenResponse>> TokenAsync(TokenRequest request)
        => await _apiClient.SendAsync<TokenResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Connect/token",
                Data = request
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.XwwwFormUrlEncoded,
                Authentication = AuthenticationType.Basic
            });

    public async ValueTask<HttpResponse<LogoutResponse>> LogoutAsync(LogoutRequest request)
        => await _apiClient.SendAsync<LogoutResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Connect/logout",
                Data = request
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> GrantConsentsAsync(GrantConsentsRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Connect/consents/grant",
                Data = request
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}