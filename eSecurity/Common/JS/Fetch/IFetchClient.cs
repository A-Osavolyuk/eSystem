namespace eSecurity.Common.JS.Fetch;

public interface IFetchClient
{
    public ValueTask<Result> FetchAsync(FetchOptions options);
}