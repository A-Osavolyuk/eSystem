using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Blazor.Server.Domain.Options;
using eShop.Domain.Common.Http;
using Microsoft.JSInterop;

namespace eShop.Blazor.Server.Infrastructure.Implementations;

public class FetchClient(IJSRuntime jSRuntime) : IFetchClient
{
    private readonly IJSRuntime jSRuntime = jSRuntime;

    public async ValueTask<HttpResponse> FetchAsync(FetchOptions options)
    {
        var body = options.Body is null ? string.Empty : JsonSerializer.Serialize(options.Body);
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
            credentials = Credentials.Include,
            body
        });
        return result;
    }
}