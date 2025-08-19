using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class OAuthService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IOAuthService
{
    public async ValueTask<Response> LoadSessionAsync(LoadOAuthSessionRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/OAuth/load", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<Response> DisconnectAsync(DisconnectLinkedAccountRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/OAuth/disconnect", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> AllowAsync(AllowLinkedAccountRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/OAuth/allow", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> DisallowAsync(DisallowLinkedAccountRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/OAuth/disallow", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> ConfirmDisconnectAsync(ConfirmDisconnectLinkedAccountRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/OAuth/confirm-disconnect", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> ConfirmAllowAsync(ConfirmAllowLinkedAccountRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/OAuth/confirm-allow", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> ConfirmDisallowAsync(ConfirmDisallowLinkedAccountRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/OAuth/confirm-disallow", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });
}