using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Blazor.Domain.Interfaces;

public interface IProvidersService
{
    public ValueTask<HttpResponse> GetProvidersAsync();
    public ValueTask<HttpResponse> SubscribeAsync(SubscribeProviderRequest request);
    public ValueTask<HttpResponse> UnsubscribeAsync(UnsubscribeProviderRequest request);
}