using eAccount.Common.Http;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eAccount.Security.Authentication.SSO;

public class SsoService(    
    IApiClient apiClient) : ISsoService
{
    private const string BasePath = "api/v1/Sso";
    private readonly IApiClient apiClient = apiClient;
    
    public async ValueTask<HttpResponse> TokenAsync(TokenRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/token", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/authorize", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
    
    public async ValueTask<HttpResponse> SignOutAsync(SignOutRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/sign-out", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });
}