using eShop.Domain.Common.Http;
using eShop.Domain.Requests.Auth;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface IProviderService
{
    public ValueTask<HttpResponse> GetProvidersAsync();
    public ValueTask<HttpResponse> SubscribeAsync(SubscribeProviderRequest request);
    public ValueTask<HttpResponse> UnsubscribeAsync(UnsubscribeProviderRequest request);
}