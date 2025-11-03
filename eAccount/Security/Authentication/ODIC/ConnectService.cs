using eAccount.Common.Http;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eAccount.Security.Authentication.ODIC;

public class ConnectService(    
    IApiClient apiClient) : IConnectService
{
    private const string BasePath = "api/v1/Connect";
    private readonly IApiClient apiClient = apiClient;
    
    public async ValueTask<HttpResponse> TokenAsync(TokenRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/token", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/authorize", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
    
    public async ValueTask<HttpResponse> LogoutAsync(LogoutRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/logout", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });
}