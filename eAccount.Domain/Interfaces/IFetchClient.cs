using eAccount.Domain.Options;
using eSystem.Core.Common.Http;

namespace eAccount.Domain.Interfaces;

public interface IFetchClient
{
    public ValueTask<HttpResponse> FetchAsync(FetchOptions options);
}