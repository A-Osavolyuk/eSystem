using eSecurity.Client.Common.Http;
using eSecurity.Client.Services.Interfaces;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Services.Implementations;

public class ConnectService(IApiClient apiClient) : IConnectService
{
    private readonly IApiClient _apiClient = apiClient;
    
    public async ValueTask<HttpResponse> GetPublicKeysAsync()
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = "api/v1/Connect/.well-known/jwks.json",
            }, new HttpOptions() { Type = DataType.Text, Wrap = true });

    public async ValueTask<HttpResponse> GetOpenidConfigurationAsync()
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Get,
                Url = "api/v1/Connect/.well-known/openid-configuration",
            }, new HttpOptions() { Type = DataType.Text, Wrap = true });

    public async ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Connect/authorize",
                Data = request
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> TokenAsync(TokenRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Connect/token",
                Data = request
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> LogoutAsync(LogoutRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Connect/logout",
                Data = request
            }, new HttpOptions() { Type = DataType.Text });
}