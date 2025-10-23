using eAccount.Blazor.Server.Domain.Abstraction.Services;
using eAccount.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.Auth;

namespace eAccount.Blazor.Server.Infrastructure.Implementations;

public class OAuthService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IOAuthService
{
    private const string BasePath = "api/v1/OAuth";
    
    public async ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/load", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/disconnect", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });
}