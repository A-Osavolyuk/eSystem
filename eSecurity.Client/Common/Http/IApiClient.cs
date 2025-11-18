using eSystem.Core.Common.Http;

namespace eSecurity.Client.Common.Http;

public interface IApiClient
{
    public ValueTask<HttpResponse<TResponse>> SendAsync<TResponse>(HttpRequest httpRequest, HttpOptions httpOptions);
    public ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions);
}