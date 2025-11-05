namespace eSecurity.Common.JS.Fetch;

public interface IFetchClient
{
    public ValueTask<HttpResponse> FetchAsync(FetchOptions options);
}