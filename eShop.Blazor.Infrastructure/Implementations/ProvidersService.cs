using eShop.Blazor.Domain.Abstraction.Services;
using eShop.Blazor.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Blazor.Infrastructure.Implementations;

public class ProvidersService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IProvidersService
{
    public async ValueTask<HttpResponse> GetProvidersAsync() => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Providers", Method = HttpMethod.Get },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> SubscribeAsync(SubscribeProviderRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Providers/subscribe", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> UnsubscribeAsync(UnsubscribeProviderRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Providers/unsubscribe", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });
}