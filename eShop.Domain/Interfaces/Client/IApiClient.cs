using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;

namespace eShop.Domain.Interfaces.Client;

public interface IApiClient
{
    public ValueTask<Response> SendAsync(HttpRequest httpRequest, HttpOptions options);
}