using eSystem.Core.Http.Results;

namespace eSecurity.Client.Common.JS.Fetch;

public interface IFetchClient
{
    public ValueTask<Result> FetchAsync(FetchOptions options);
}