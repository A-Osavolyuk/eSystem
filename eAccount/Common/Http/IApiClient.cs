using eSystem.Core.Common.Http;

namespace eAccount.Common.Http;

public interface IApiClient
{
    public ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions);
}