using eSystem.Core.Common.Http;

namespace eSecurity.Client.Common.Http;

public interface IApiClient
{
    public ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions);
}