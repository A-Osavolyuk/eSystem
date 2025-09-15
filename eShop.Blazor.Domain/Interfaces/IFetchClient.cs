using eShop.Blazor.Domain.Options;
using eShop.Domain.Common.Http;

namespace eShop.Blazor.Domain.Interfaces;

public interface IFetchClient
{
    public ValueTask<HttpResponse> FetchAsync(FetchOptions options);
}