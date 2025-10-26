using eSystem.Core.Common.Http;

namespace eAccount.Domain.Interfaces;

public interface IApiClient
{
    public ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions);
}