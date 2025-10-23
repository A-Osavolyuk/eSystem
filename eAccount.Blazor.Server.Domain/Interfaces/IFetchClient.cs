using eAccount.Blazor.Server.Domain.Options;
using eSystem.Domain.Common.Http;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IFetchClient
{
    public ValueTask<HttpResponse> FetchAsync(FetchOptions options);
}