using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Blazor.Server.Domain.Options;
using Microsoft.JSInterop;

namespace eShop.Blazor.Server.Infrastructure.Implementations;

public class FetchClient(IJSRuntime jSRuntime) : IFetchClient
{
    private readonly IJSRuntime jSRuntime = jSRuntime;

    public async ValueTask<HttpResponse> FetchAsync(FetchOptions options)
    {
        var headers = new Dictionary<string, string>()
        {
            { "Accept", "application/json" },
            { "Content-Type", "application/json" }
        };
        
        var result = await jSRuntime.InvokeAsync<HttpResponse>("fetchApi", new
        {
            url = options.Url,
            headers,
            method = options.Method.Method,
            credentials = options.Credentials,
            body = options.Body,
        });
        return result;
    }
}