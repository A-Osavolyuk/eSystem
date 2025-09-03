using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IProvidersService
{
    public ValueTask<Response> GetProvidersAsync();
    public ValueTask<Response> SubscribeAsync(SubscribeProviderRequest request);
    public ValueTask<Response> VerifyAsync(VerifyProviderRequest request);
    public ValueTask<Response> UnsubscribeAsync(UnsubscribeProviderRequest request);
}