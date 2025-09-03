using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IProvidersService
{
    public ValueTask<HttpResponse> GetProvidersAsync();
    public ValueTask<HttpResponse> SubscribeAsync(SubscribeProviderRequest request);
    public ValueTask<HttpResponse> VerifyAsync(VerifyProviderRequest request);
    public ValueTask<HttpResponse> UnsubscribeAsync(UnsubscribeProviderRequest request);
}