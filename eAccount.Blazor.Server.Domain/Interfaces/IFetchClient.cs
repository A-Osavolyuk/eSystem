using eAccount.Blazor.Server.Domain.Options;
using eShop.Domain.Common.Http;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IFetchClient
{
    public ValueTask<HttpResponse> FetchAsync(FetchOptions options);
}