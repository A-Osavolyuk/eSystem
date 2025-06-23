using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IProvidersService
{
    public ValueTask<Response> GetProvidersAsync();
    public ValueTask<Response> SubscribeProviderAsync(SubscribeProviderRequest request);
    public ValueTask<Response> UnsubscribeProviderAsync(UnsubscribeProviderRequest request);
}