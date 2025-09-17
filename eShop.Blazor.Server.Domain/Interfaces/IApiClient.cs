using eShop.Domain.Common.Http;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface IApiClient
{
    public ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions options);
}