using eAccount.Domain.Abstraction.Services;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Requests.Auth;

namespace eAccount.Infrastructure.Implementations;

public class OAuthService(
    IApiClient apiClient) : IOAuthService
{
    private readonly IApiClient apiClient = apiClient;
    private const string BasePath = "api/v1/OAuth";
    
    public async ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/load", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/disconnect", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });
}