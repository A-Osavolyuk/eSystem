using eSystem.Core.Common.Http;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IApiClient
{
    public ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions);
}