using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class ProvidersService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IProvidersService
{
    public async ValueTask<Response> GetProvidersAsync() => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Providers", Method = HttpMethod.Get },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> SubscribeProviderAsync(SubscribeProviderRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Providers/subscribe", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> UnsubscribeProviderAsync(UnsubscribeProviderRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Providers/unsubscribe", Method = HttpMethod.Post, Data = request },
        new HttpOptions { ValidateToken = true, WithBearer = true, Type = DataType.Text });
}