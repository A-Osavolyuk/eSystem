using eSystem.Core.Common.Http;

namespace eSecurity.Client.Common.Http;

public interface IApiClient
{
    public ValueTask<Result> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions);
}