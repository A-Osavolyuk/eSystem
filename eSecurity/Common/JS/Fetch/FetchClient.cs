using System.Text.Json;
using eSystem.Core.Common.Http;
using Microsoft.JSInterop;

namespace eSecurity.Common.JS.Fetch;

public class FetchClient(IJSRuntime jSRuntime) : IFetchClient
{
    private readonly IJSRuntime jSRuntime = jSRuntime;

    public async ValueTask<Result> FetchAsync(FetchOptions options)
    {
        var body = options.Body is null ? string.Empty : JsonSerializer.Serialize(options.Body);
        var headers = new Dictionary<string, string>()
        {
            { "Accept", "application/json" },
            { "Content-Type", "application/json" }
        };
        
        return await jSRuntime.InvokeAsync<Result>("fetchApi", new
        {
            url = options.Url,
            headers,
            method = options.Method.Method,
            credentials = Credentials.Include,
            body,
        });
    }
}