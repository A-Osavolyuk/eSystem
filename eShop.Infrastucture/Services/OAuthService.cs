using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class OAuthService(IConfiguration configuration, IApiClient apiClient) : ApiService(configuration, apiClient), IOAuthService
{
    public async ValueTask<Response> LoadSessionAsync(LoadOAuthSessionRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/OAuth/load", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });
}