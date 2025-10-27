using eAccount.Domain.Options;
using Microsoft.JSInterop;

namespace eAccount.Infrastructure.Implementations;

public class FetchClient(IJSRuntime jSRuntime) : IFetchClient
{
    private readonly IJSRuntime jSRuntime = jSRuntime;

    public async ValueTask<HttpResponse> FetchAsync(FetchOptions options)
    {
        var body = JsonSerializer.Serialize(options.Body);
        var headers = new Dictionary<string, string>()
        {
            { "Accept", "application/json" },
            { "Content-Type", "application/json" }
        };
        
        return await jSRuntime.InvokeAsync<HttpResponse>("fetchApi", new
        {
            url = options.Url,
            headers,
            method = options.Method.Method,
            credentials = Credentials.Include,
            body,
        });
    }
}