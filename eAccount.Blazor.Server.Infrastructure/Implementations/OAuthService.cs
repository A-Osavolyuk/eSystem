using eAccount.Blazor.Server.Domain.Abstraction.Services;

namespace eAccount.Blazor.Server.Infrastructure.Implementations;

public class OAuthService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IOAuthService
{
    private const string BasePath = "api/v1/OAuth";
    
    public async ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/load", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> DisconnectAsync(DisconnectLinkedAccountRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/disconnect", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
}