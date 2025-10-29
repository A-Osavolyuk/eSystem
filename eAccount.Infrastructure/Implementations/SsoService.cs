using eAccount.Domain.Abstraction.Services;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Requests.Auth;

namespace eAccount.Infrastructure.Implementations;

public class SsoService(    
    GatewayOptions gatewayOptions,
    IApiClient apiClient) : ISsoService
{
    private const string BasePath = "api/v1/Sso";
    
    public async ValueTask<HttpResponse> TokenAsync(TokenRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/token", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> AuthorizeAsync(AuthorizeRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/authorize", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
    
    public async ValueTask<HttpResponse> UnauthorizeAsync(SignOutRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gatewayOptions.Url}/{BasePath}/unauthorize", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
}